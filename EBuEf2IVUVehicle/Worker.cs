using Common.Enums;
using Common.EventsArgs;
using Common.Interfaces;
using Common.Models;
using EBuEf2IVUVehicle.Settings;
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
using System.Threading;
using System.Threading.Tasks;

namespace EBuEf2IVUVehicle
{
    internal class Worker
        : BackgroundService
    {
        #region Private Fields

        private const string MessageTypePositions = "Echtzeit-Positionen";

        private readonly IConfiguration config;
        private readonly IDatabaseConnector databaseConnector;
        private readonly IEnumerable<InfrastructureMapping> infrastructureMappings;
        private readonly IRealtimeSender ivuSender;
        private readonly ILogger logger;
        private readonly IMessageReceiver positionsReceiver;
        private readonly JsonSerializerSettings positionsReceiverSettings = new JsonSerializerSettings();
        private readonly IStateHandler sessionStateHandler;

        private DateTime ebuefSessionStart = DateTime.Now;
        private DateTime ivuSessionDate = DateTime.Now;
        private CancellationTokenSource sessionCancellationTokenSource;

        #endregion Private Fields

        #region Public Constructors

        public Worker(IConfiguration config, ILogger<Worker> logger, IStateHandler sessionStateHandler,
            IDatabaseConnector databaseConnector)
        {
            this.config = config;
            this.logger = logger;

            var assemblyInfo = Assembly.GetExecutingAssembly().GetName();
            logger.LogInformation($"{assemblyInfo.Name} (Version {assemblyInfo.Version.Major}.{assemblyInfo.Version.Minor}) wird gestartet.");

            ivuSender = GetSender();
            infrastructureMappings = GetInfrastructureMappings();

            positionsReceiver = GetPositionReceiver();
            positionsReceiver.MessageReceivedEvent += OnPositionReceived;

            this.databaseConnector = databaseConnector;

            this.sessionStateHandler = sessionStateHandler;
            this.sessionStateHandler.SessionStartedEvent += OnSessionStartedAsync;
            this.sessionStateHandler.SessionChangedEvent += OnSessionChangedAsync;
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override async Task ExecuteAsync(CancellationToken workerCancellationToken)
        {
            while (!workerCancellationToken.IsCancellationRequested)
            {
                var sessionCancellationToken = GetSessionCancellationToken(workerCancellationToken);

                InitializeConnector(sessionCancellationToken);
                InitializeStateHandler();

                await StartIVUSessionAsync();

                while (!sessionCancellationToken.IsCancellationRequested)
                {
                    await Task.WhenAny(
                        sessionStateHandler.RunAsync(sessionCancellationToken),
                        positionsReceiver.RunAsync(sessionCancellationToken),
                        ivuSender.RunAsnc(sessionCancellationToken));
                };
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private IEnumerable<string> GetDecoders(RealTimeMessage message)
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

                if ((message.SignalTyp == SignalTyp.ESig && mapping.IVUTrainPositionType != TrainPositionType.Ankunft)
                    || (message.SignalTyp == SignalTyp.ASig && mapping.IVUTrainPositionType != TrainPositionType.Abfahrt)
                    || (message.SignalTyp == SignalTyp.BkSig && mapping.IVUTrainPositionType != TrainPositionType.Durchfahrt))
                {
                    logger.LogWarning($"Der IVUTrainPositionType des Mappings ({mapping.IVUTrainPositionType}) entspricht " +
                        $"nicht dem SignalTyp der eingegangenen Nachricht ({message.ToString()}).");
                }

                result = new TrainPosition
                {
                    EBuEfBetriebsstelleNach = mapping.EBuEfNachBetriebsstelle,
                    EBuEfBetriebsstelleVon = mapping.EBuEfVonBetriebsstelle,
                    EBuEfGleisNach = message.EndGleis,
                    EBuEfGleisVon = message.StartGleis,
                    EBuEfZeitpunktNach = message.SimulationsZeit.Add(ebuefNachZeit).TimeOfDay,
                    EBuEfZeitpunktVon = message.SimulationsZeit.Add(ebuefVonZeit).TimeOfDay,
                    Fahrzeuge = GetDecoders(message).ToArray(),
                    IVUGleis = mapping.IVUGleis,
                    IVUNetzpunkt = mapping.IVUNetzpunkt,
                    IVUTrainPositionTyp = mapping.IVUTrainPositionType,
                    IVUZeitpunkt = ivuZeitpunkt,
                    Zugnummer = message.Zugnummer,
                };
            }

            return result;
        }

        private IMessageReceiver GetPositionReceiver()
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

        private IRealtimeSender GetSender()
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

        private void InitializeConnector(CancellationToken sessionCancellationToken)
        {
            var settings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            databaseConnector.Initialize(
                connectionString: settings.ConnectionString,
                retryTime: settings.RetryTime,
                cancellationToken: sessionCancellationToken);
        }

        private void InitializeStateHandler()
        {
            var settings = config
                .GetSection(nameof(StatusReceiver))
                .Get<StatusReceiver>();

            sessionStateHandler.Initialize(
                ipAdress: settings.IpAddress,
                port: settings.ListenerPort,
                retryTime: settings.RetryTime,
                startPattern: settings.StartPattern,
                statusPattern: settings.StatusPattern);
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
                    else
                    {
                        logger.LogDebug($"Zu der gesandten Echtzeitmeldung konnte " +
                            $"in der aktuellen Sitzung keine Fahrt gefunden werden.");
                    }
                }
            }
            catch (JsonReaderException readerException)
            {
                logger.LogError($"Die Nachricht kann nicht gelesen werden: {readerException.Message}");
            }
            catch (JsonSerializationException serializationException)
            {
                logger.LogError($"Die Nachricht kann nicht in eine Echtzeitmeldung " +
                    $"umgeformt werden: {serializationException.Message}");
            }
        }

        private async void OnSessionChangedAsync(object sender, StateChangedArgs e)
        {
            if (e.State == SessionStates.IsRunning)
            {
                logger?.LogInformation("Sessionstart-Nachricht empfangen.");

                await StartIVUSessionAsync();
                await SetVehicleAllocationsAsync();
            }
            else if (e.State == SessionStates.IsPaused)
            {
                logger.LogInformation("Sessionpause-Nachricht empfangen. Die Nachrichtenempfänger, " +
                    "Datenbank-Verbindungen und IVU-Sender werden zurückgesetzt.");

                sessionCancellationTokenSource.Cancel();
            }
        }

        private async void OnSessionStartedAsync(object sender, EventArgs e)
        {
            logger?.LogInformation("Sessionstart-Nachricht empfangen.");

            await StartIVUSessionAsync();
            await SetVehicleAllocationsAsync();
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

            ivuSessionDate = currentSession.IVUDatum;
            ebuefSessionStart = ivuSessionDate
                .Add(currentSession.SessionStart.TimeOfDay);

            logger.LogDebug($"Die IVU-Sitzung beginnt am {ivuSessionDate:yyyy-MM-dd} um " +
                $"{ebuefSessionStart:hh:mm:ss}.");
        }

        #endregion Private Methods
    }
}