using Common.EventsArgs;
using Common.Interfaces;
using Common.Models;
using DatabaseConnector;
using EBuEf2IVUCore.Settings;
using Extensions;
using MessageReceiver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RealtimeSender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EBuEf2IVUCore
{
    public class Worker
        : BackgroundService
    {
        #region Private Fields

        private const string MessageTypeAllocations = "Sessionstatus";
        private const string MessageTypePositions = "Echtzeit-Positionen";
        private const string StatusRegexGroupName = "status";
        private const string StatusRegexGroupWildcard = "$";

        private readonly IConfiguration config;
        private readonly IEnumerable<InfrastructureMapping> infrastructureMappings;
        private readonly ISender ivuSender;
        private readonly ILogger<Worker> logger;
        private readonly IReceiver positionsReceiver;
        private readonly JsonSerializerSettings positionsReceiverSettings;
        private readonly Regex startRegex;
        private readonly IReceiver statusReceiver;
        private readonly Regex statusRegex;

        private IConnector databaseConnector;
        private DateTime ebuefSessionStart = DateTime.Now;
        private DateTime ivuSessionDate = DateTime.Now;
        private CancellationTokenSource sessionCancellationTokenSource;

        #endregion Private Fields

        #region Public Constructors

        public Worker(IConfiguration config, ILogger<Worker> logger)
        {
            this.config = config;
            this.logger = logger;

            var assemblyInfo = Assembly.GetExecutingAssembly().GetName();
            logger.LogInformation($"{assemblyInfo.Name} (Version {assemblyInfo.Version.Major}.{assemblyInfo.Version.Minor}) wird gestartet.");

            ivuSender = GetSender();
            infrastructureMappings = GetInfrastructureMappings();

            statusReceiver = GetStatusReceiver();
            statusReceiver.MessageReceivedEvent += OnStatusReceived;
            statusRegex = GetStatusRegex();
            startRegex = GetStartRegex();

            positionsReceiver = GetPositionReceiver();
            positionsReceiver.MessageReceivedEvent += OnPositionReceived;
            positionsReceiverSettings = GetPositionsReceiverSettings();
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override async Task ExecuteAsync(CancellationToken workerCancellationToken)
        {
            while (!workerCancellationToken.IsCancellationRequested)
            {
                var sessionCancellationToken = GetSessionCancellationToken(workerCancellationToken);
                databaseConnector = GetConnector(sessionCancellationToken);

                await StartIVUSessionAsync();

                while (!sessionCancellationToken.IsCancellationRequested)
                {
                    await Task.WhenAny(
                        statusReceiver.RunAsync(sessionCancellationToken),
                        positionsReceiver.RunAsync(sessionCancellationToken),
                        ivuSender.RunAsnc(sessionCancellationToken));
                };
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private static JsonSerializerSettings GetPositionsReceiverSettings()
        {
            return new JsonSerializerSettings();
        }

        private IConnector GetConnector(CancellationToken sessionCancellationToken)
        {
            var settings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            logger.LogDebug($"Verbindung zur Datenbank herstellen: {settings.ConnectionString}.");

            var result = new Connector(
                logger: logger,
                connectionString: settings.ConnectionString,
                retryTime: settings.RetryTime,
                cancellationToken: sessionCancellationToken);

            return result;
        }

        private IEnumerable<string> GetFahrzeuge(RealTimeMessage message)
        {
            if (!string.IsNullOrWhiteSpace(message?.Decoder))
                yield return message.Decoder;
        }

        private IEnumerable<InfrastructureMapping> GetInfrastructureMappings()
        {
            var result = config
                .GetSection(nameof(InfrastructureMappings))
                .Get<InfrastructureMappings>()
                .InfrastructureMapping;

            return result;
        }

        private TrainPosition GetPosition(RealTimeMessage message)
        {
            var result = default(TrainPosition);

            var mapping = infrastructureMappings
                .Where(m => m.MessageBetriebsstelle.IsMatch(message.Betriebsstelle))
                .Where(m => m.MessageStartGleis.IsMatchOrEmptyOrIgnored(message.StartGleis)
                    && m.MessageEndGleis.IsMatchOrEmptyOrIgnored(message.EndGleis))
                .OrderByDescending(m => m.MessageStartGleis.IsMatch(message.StartGleis))
                .ThenByDescending(m => m.MessageEndGleis.IsMatch(message.EndGleis))
                .FirstOrDefault();

            if (mapping != null)
            {
                var ebuefVonZeit = TimeSpan.FromSeconds(mapping.EBuEfVonVerschiebungSekunden.ToInt());
                var ebuefNachZeit = TimeSpan.FromSeconds(mapping.EBuEfNachVerschiebungSekunden.ToInt());
                var ivuZeit = TimeSpan.FromSeconds(mapping.IVUVerschiebungSekunden.ToInt());

                var ivuZeitpunkt = ivuSessionDate.Add(message.SimulationsZeit.Add(ivuZeit).TimeOfDay);

                result = new TrainPosition
                {
                    EBuEfBetriebsstelleNach = mapping.EBuEfNachBetriebsstelle,
                    EBuEfBetriebsstelleVon = mapping.EBuEfVonBetriebsstelle,
                    EBuEfGleisNach = message.EndGleis,
                    EBuEfGleisVon = message.StartGleis,
                    EBuEfZeitpunktNach = message.SimulationsZeit.Add(ebuefNachZeit).TimeOfDay,
                    EBuEfZeitpunktVon = message.SimulationsZeit.Add(ebuefVonZeit).TimeOfDay,
                    Fahrzeuge = GetFahrzeuge(message).ToArray(),
                    IVUGleis = mapping.IVUGleis,
                    IVUNetzpunkt = mapping.IVUNetzpunkt,
                    IVUTrainPositionTyp = mapping.IVUTrainPositionType,
                    IVUZeitpunkt = ivuZeitpunkt,
                    Zugnummer = message.Zugnummer,
                };
            }

            return result;
        }

        private IReceiver GetPositionReceiver()
        {
            var settings = config
                .GetSection(nameof(PositionsReceiver))
                .Get<PositionsReceiver>();

            var result = new Receiver(
                ipAdress: settings.IpAddress,
                port: settings.ListenerPort,
                retryTime: settings.RetryTime,
                logger: logger,
                messageType: MessageTypePositions);

            return result;
        }

        private ISender GetSender()
        {
            var settings = config
                .GetSection(nameof(IVURealtimeSender))
                .Get<IVURealtimeSender>();

            var result = new Sender(
                logger: logger,
                division: settings.Division,
                endpoint: settings.Endpoint,
                retryTime: settings.RetryTime);

            return result;
        }

        private CancellationToken GetSessionCancellationToken(CancellationToken workerCancellationToken)
        {
            sessionCancellationTokenSource = new CancellationTokenSource();
            workerCancellationToken.Register(() => sessionCancellationTokenSource.Cancel());

            return sessionCancellationTokenSource.Token;
        }

        private Regex GetStartRegex()
        {
            var settings = config
                .GetSection(nameof(StatusReceiver))
                .Get<StatusReceiver>();

            var result = new Regex(settings.Pattern);

            return result;
        }

        private IReceiver GetStatusReceiver()
        {
            var settings = config
                .GetSection(nameof(StatusReceiver))
                .Get<StatusReceiver>();

            var result = new Receiver(
                ipAdress: settings.IpAddress,
                port: settings.ListenerPort,
                retryTime: settings.RetryTime,
                logger: logger,
                messageType: MessageTypeAllocations);

            return result;
        }

        private Regex GetStatusRegex()
        {
            var settings = config
                .GetSection(nameof(StatusReceiver))
                .Get<StatusReceiver>();

            var statusPattern = settings.StatusPattern.Replace(
                oldValue: StatusRegexGroupWildcard,
                newValue: $@"(?<{StatusRegexGroupName}>\d)");

            var result = new Regex(statusPattern);

            return result;
        }

        private void OnPositionReceived(object sender, MessageReceivedArgs e)
        {
            logger.LogDebug($"Positions-Nachricht empfangen: {e.Content}");

            try
            {
                var message = JsonConvert.DeserializeObject<RealTimeMessage>(
                    value: e.Content,
                    settings: positionsReceiverSettings);

                if (!string.IsNullOrWhiteSpace(message?.Zugnummer))
                {
                    var position = GetPosition(message);

                    if (position != null)
                    {
                        databaseConnector.AddRealtimeAsync(position);
                        ivuSender.AddRealtime(position);
                    }
                }
            }
            catch (JsonReaderException readerException)
            {
                logger.LogError($"Die Nachricht kann nicht in eine Echtzeitmeldung " +
                    $"umgeformt werden: {readerException.Message}");
            }
        }

        private async void OnStatusReceived(object sender, MessageReceivedArgs e)
        {
            logger.LogDebug($"Status-Nachricht empfangen: {e.Content}");

            if (startRegex.IsMatch(e.Content) || statusRegex.IsMatch(e.Content))
            {
                var sessionStatus = statusRegex.IsMatch(e.Content)
                    ? statusRegex.Match(e.Content).Groups[StatusRegexGroupName].Value
                    : default;

                if (startRegex.IsMatch(e.Content)
                    || sessionStatus == Connector.SessionIsRunning.ToString())
                {
                    logger.LogInformation("Sessionstart-Nachricht empfangen.");

                    await StartIVUSessionAsync();
                    await SetVehicleAllocationsAsync();
                }
                else if (sessionStatus == Connector.SessionIsPaused.ToString())
                {
                    logger.LogInformation("Sessionpause-Nachricht empfangen. Die Nachrichtenempfänger, " +
                        "Datenbank-Verbindungen und IVU-Sender werden zurückgesetzt.");

                    sessionCancellationTokenSource.Cancel();
                }
                else
                {
                    logger.LogError($"Unbekannten Sessionstatus empfangen: {sessionStatus}.");
                }
            }
            else
            {
                logger.LogError($"Unbekannte Status-Nachricht empfangen: '{e.Content}'.");
            }
        }

        private async Task SetVehicleAllocationsAsync()
        {
            var allocations = await databaseConnector.GetVehicleAllocationsAsync();
            ivuSender.AddAllocations(
                allocations: allocations,
                startTime: ebuefSessionStart);
        }

        private async Task StartIVUSessionAsync()
        {
            var currentSession = await databaseConnector.GetEBuEfSessionAsync();

            ivuSessionDate = currentSession.IVUDate;
            ebuefSessionStart = ivuSessionDate.Add(currentSession.SessionStart.TimeOfDay);

            logger.LogDebug($"IVU-Sitzung beginnt am {ivuSessionDate:yyyy-MM-dd} um " +
                $"{ebuefSessionStart:hh:mm:ss}.");
        }

        #endregion Private Methods
    }
}