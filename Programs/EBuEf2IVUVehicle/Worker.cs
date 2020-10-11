#pragma warning disable CA1031 // Do not catch general exception types

using Common.Enums;
using Common.EventsArgs;
using Common.Interfaces;
using Common.Models;
using ConverterExtensions;
using EBuEf2IVUBase;
using EBuEf2IVUVehicle.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RegexExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EBuEf2IVUVehicle
{
    internal class Worker
        : WorkerBase
    {
        #region Private Fields

        private const string MessageTypePositions = "Echtzeit-Positionen";

        private readonly IEnumerable<InfrastructureMapping> infrastructureMappings;
        private readonly IRealtimeSender ivuSender;
        private readonly IMessageReceiver positionsReceiver;
        private readonly JsonSerializerSettings positionsReceiverSettings = new JsonSerializerSettings();

        #endregion Private Fields

        #region Public Constructors

        public Worker(IConfiguration config, IStateHandler sessionStateHandler, IMessageReceiver positionsReceiver,
            IDatabaseConnector databaseConnector, IRealtimeSender ivuSender, ILogger<Worker> logger)
            : base(config, sessionStateHandler, databaseConnector, logger)
        {
            infrastructureMappings = GetInfrastructureMappings();

            this.sessionStateHandler.AllocationSetEvent += OnAllocationSet;

            this.positionsReceiver = positionsReceiver;
            this.positionsReceiver.MessageReceivedEvent += OnPositionReceived;

            this.ivuSender = ivuSender;
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override async Task ExecuteAsync(CancellationToken workerCancellationToken)
        {
            InitializeStateHandler();
            sessionStateHandler.Run(workerCancellationToken);

            while (!workerCancellationToken.IsCancellationRequested)
            {
                logger.LogInformation(
                    "Die Nachrichtenempfänger, Datenbank-Verbindungen und IVU-Sender von EBuEf2IVUVehicle werden zurückgesetzt.");

                var sessionCancellationToken = GetSessionCancellationToken(workerCancellationToken);

                InitializePositionReceiver();
                InitializeConnector(sessionCancellationToken);
                InitializeSender();

                await StartIVUSessionAsync();

                while (!sessionCancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.WhenAny(
                            positionsReceiver.RunAsync(sessionCancellationToken),
                            ivuSender.RunAsnc(sessionCancellationToken));
                    }
                    catch (TaskCanceledException)
                    {
                        logger.LogInformation(
                            "EBuEf2IVUVehicle wird beendet.");
                    }
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

        private TrainLeg GetLeg(RealTimeMessage message)
        {
            var result = default(TrainLeg);

            var mapping = infrastructureMappings
                .Where(m => m.MessageBetriebsstelle.IsMatch(message.Betriebsstelle))
                .Where(m => m.MessageStartGleis.IsMatchOrEmptyPatternOrEmptyValue(message.StartGleis)
                    && m.MessageEndGleis.IsMatchOrEmptyPatternOrEmptyValue(message.EndGleis))
                .OrderByDescending(m => m.MessageStartGleis.IsMatch(message.StartGleis))
                .ThenByDescending(m => m.MessageEndGleis.IsMatch(message.EndGleis))
                .FirstOrDefault();

            if (mapping != null)
            {
                var ebuefVonZeit = TimeSpan.FromSeconds(mapping.EBuEfVonVerschiebungSekunden.ToInt());
                var ebuefNachZeit = TimeSpan.FromSeconds(mapping.EBuEfNachVerschiebungSekunden.ToInt());
                var ivuZeit = TimeSpan.FromSeconds(mapping.IVUVerschiebungSekunden.ToInt());

                var ivuZeitpunkt = ivuSessionDate.Add(message.SimulationsZeit.Add(ivuZeit).TimeOfDay);

                if ((message.SignalTyp == SignalType.ESig && mapping.IVUTrainPositionType != LegType.Ankunft)
                    || (message.SignalTyp == SignalType.ASig && mapping.IVUTrainPositionType != LegType.Abfahrt)
                    || (message.SignalTyp == SignalType.BkSig && mapping.IVUTrainPositionType != LegType.Durchfahrt))
                {
                    logger.LogWarning(
                        "Der IVUTrainPositionType des Mappings {mappingType} entspricht nicht dem SignalTyp der eingegangenen Nachricht ({message}).",
                        mapping.IVUTrainPositionType,
                        message);
                }

                result = new TrainLeg
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
                    IVULegTyp = mapping.IVUTrainPositionType,
                    IVUZeitpunkt = ivuZeitpunkt,
                    Zugnummer = message.Zugnummer,
                };
            }

            return result;
        }

        private void InitializePositionReceiver()
        {
            var settings = config
                .GetSection(nameof(PositionsReceiver))
                .Get<PositionsReceiver>();

            positionsReceiver.Initialize(
                ipAdress: settings.IpAddress,
                port: settings.ListenerPort,
                retryTime: settings.RetryTime,
                messageType: MessageTypePositions);
        }

        private void InitializeSender()
        {
            var settings = config
                .GetSection(nameof(IVURealtimeSender))
                .Get<IVURealtimeSender>();

            ivuSender.Initialize(
                division: settings.Division,
                endpoint: settings.Endpoint,
                retryTime: settings.RetryTime);
        }

        private async void OnAllocationSet(object sender, EventArgs e)
        {
            logger.LogInformation(
                "Nachrichte zur fertigen Fahrzeugzuteilung empfangen.");

            await SetVehicleAllocationsAsync();
        }

        private void OnPositionReceived(object sender, MessageReceivedArgs e)
        {
            logger.LogDebug(
                "Positions-Nachricht empfangen: {content}",
                e.Content);

            try
            {
                var message = JsonConvert.DeserializeObject<RealTimeMessage>(
                    value: e.Content,
                    settings: positionsReceiverSettings);

                if (!string.IsNullOrWhiteSpace(message?.Zugnummer))
                {
                    var position = GetLeg(message);

                    if (position != null)
                    {
                        databaseConnector.AddRealtimeAsync(position);
                        ivuSender.AddRealtime(position);
                    }
                    else
                    {
                        logger.LogDebug(
                            "Zu der gesandten Echtzeitmeldung konnte in der aktuellen Sitzung keine Fahrt gefunden werden.");
                    }
                }
            }
            catch (JsonReaderException readerException)
            {
                logger.LogError(
                    "Die Nachricht kann nicht gelesen werden: {message}",
                    readerException.Message);
            }
            catch (JsonSerializationException serializationException)
            {
                logger.LogError(
                    "Die Nachricht kann nicht in eine Echtzeitmeldung umgeformt werden: {message}",
                    serializationException.Message);
            }
        }

        private async Task SetVehicleAllocationsAsync()
        {
            var allocations = await databaseConnector.GetVehicleAllocationsAsync();

            ivuSender.AddAllocations(
                allocations: allocations,
                startTime: ebuefSessionStart);
        }

        #endregion Private Methods
    }
}

#pragma warning disable CA1031 // Do not catch general exception types