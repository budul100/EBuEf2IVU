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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace EBuEf2IVUCore
{
    public class Worker
        : BackgroundService
    {
        #region Private Fields

        private const string MessageTypeAllocations = "Sessionstart";
        private const string MessageTypePositions = "Echtzeit-Positionen";

        private readonly IReceiver allocationsReceiver;
        private readonly Regex allocationsRegex;
        private readonly IConfiguration config;
        private readonly IEnumerable<InfrastructureMapping> infrastructureMappings;
        private readonly ISender ivuSender;
        private readonly ILogger<Worker> logger;
        private readonly IReceiver positionsReceiver;
        private readonly JsonSerializerSettings positionsReceiverSettings;

        private IConnector databaseConnector;
        private DateTime sessionDate;

        #endregion Private Fields

        #region Public Constructors

        public Worker(IConfiguration config, ILogger<Worker> logger)
        {
            this.config = config;
            this.logger = logger;

            ivuSender = GetSender();
            infrastructureMappings = GetInfrastructureMappings();

            allocationsReceiver = GetReceiverAllocations();
            allocationsReceiver.MessageReceivedEvent += OnSessionStartReceived;
            allocationsRegex = GetAllocationsRegex();

            positionsReceiver = GetReceiverPositions();
            positionsReceiver.MessageReceivedEvent += OnPositionsReceived;
            positionsReceiverSettings = GetPositionsReceiverSettings();
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Run(async () => await ExecuteServices(cancellationToken));
        }

        #endregion Protected Methods

        #region Private Methods

        private static JsonSerializerSettings GetPositionsReceiverSettings()
        {
            return new JsonSerializerSettings();
        }

        private async Task ExecuteServices(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                databaseConnector = GetConnector(cancellationToken);

                await Task.WhenAny(
                    allocationsReceiver.RunAsync(cancellationToken),
                    positionsReceiver.RunAsync(cancellationToken),
                    ivuSender.RunAsnc(cancellationToken));
            };
        }

        private Regex GetAllocationsRegex()
        {
            var settings = config
                .GetSection(nameof(AllocationsReceiver))
                .Get<AllocationsReceiver>();

            var result = new Regex(settings.Pattern);

            return result;
        }

        private IConnector GetConnector(CancellationToken cancellationToken)
        {
            var settings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            var result = new Connector(
                logger: logger,
                connectionString: settings.ConnectionString,
                retryTime: settings.RetryTime,
                cancellationToken: cancellationToken);

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

        private IReceiver GetReceiverAllocations()
        {
            var settings = config
                .GetSection(nameof(AllocationsReceiver))
                .Get<AllocationsReceiver>();

            var result = new Receiver(
                ipAdress: settings.IpAddress,
                port: settings.ListenerPort,
                retryTime: settings.RetryTime,
                logger: logger,
                messageType: MessageTypeAllocations);

            return result;
        }

        private IReceiver GetReceiverPositions()
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

        private TrainPosition GetTrainPosition(RealTimeMessage message)
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

                var ivuZeitpunkt = sessionDate.Add(message.SimulationsZeit.Add(ivuZeit).TimeOfDay);

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

        private void OnPositionsReceived(object sender, MessageReceivedArgs e)
        {
            logger.LogDebug($"Message received: {e.Content}");

            var message = default(RealTimeMessage);

            try
            {
                message = JsonConvert.DeserializeObject<RealTimeMessage>(
                    value: e.Content,
                    settings: positionsReceiverSettings);
            }
            catch (JsonReaderException readerException)
            {
                logger.LogError($"Received message cannot be converted into a real-time message: {readerException.Message}");
            }

            if (!string.IsNullOrWhiteSpace(message?.Zugnummer))
            {
                var position = GetTrainPosition(message);

                if (position != null)
                {
                    databaseConnector.AddRealtimeAsync(position);
                    ivuSender.AddRealtime(position);
                }
            }
        }

        private async void OnSessionStartReceived(object sender, MessageReceivedArgs e)
        {
            if (allocationsRegex.IsMatch(e.Content))
            {
                sessionDate = await databaseConnector.GetSessionDateAsync();
                logger.LogInformation($"Die Daten werden nach IVU für den {sessionDate:yyyy-MM-dd} gesendet.");

                var allocations = await databaseConnector.GetVehicleAllocationsAsync();
                ivuSender.AddAllocations(allocations);
            }
            else
            {
                logger.LogError($"Unknown session start command received: '{e.Content}'.");
            }
        }

        #endregion Private Methods
    }
}