#pragma warning disable CA1031 // Do not catch general exception types

using Common.EventsArgs;
using Common.Interfaces;
using Common.Models;
using EBuEf2IVUBase;
using EBuEf2IVUVehicle.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EBuEf2IVUVehicle
{
    internal class Worker
        : WorkerBase
    {
        #region Private Fields

        private const string MessageTypePositions = "Echtzeit-Positionen";

        private readonly Message2TrainLeg converter;
        private readonly IMessageReceiver positionsReceiver;
        private readonly IRealtimeSender realtimeSender;

        #endregion Private Fields

        #region Public Constructors

        public Worker(IConfiguration config, IStateHandler sessionStateHandler, IMessageReceiver positionsReceiver,
            IDatabaseConnector databaseConnector, IRealtimeSender realtimeSender, ILogger logger)
            : base(config, sessionStateHandler, databaseConnector, logger)
        {
            this.sessionStateHandler.SessionStartedEvent += OnSessionStart;

            this.positionsReceiver = positionsReceiver;
            this.positionsReceiver.MessageReceivedEvent += OnMessageReceived;

            this.realtimeSender = realtimeSender;

            converter = new Message2TrainLeg(
                config: config,
                logger: logger,
                ivuSessionDate: ivuSessionDate);
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override async Task ExecuteAsync(CancellationToken workerCancellationToken)
        {
            InitializeStateReceiver();
            sessionStateHandler.Run(workerCancellationToken);

            while (!workerCancellationToken.IsCancellationRequested)
            {
                logger.LogInformation(
                    "Die Nachrichtenempfänger, Datenbank-Verbindungen und IVU-Sender von EBuEf2IVUVehicle werden zurückgesetzt.");

                var sessionCancellationToken = GetSessionCancellationToken(workerCancellationToken);

                InitializePositionReceiver();
                InitializeRealtimeSender();

                InitializeDatabaseConnector(sessionCancellationToken);

                await StartIVUSessionAsync();

                while (!sessionCancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.WhenAny(
                            positionsReceiver.RunAsync(sessionCancellationToken),
                            realtimeSender.RunAsnc(sessionCancellationToken));
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

        private void InitializePositionReceiver()
        {
            var settings = config
                .GetSection(nameof(PositionsReceiver))
                .Get<PositionsReceiver>();

            positionsReceiver.Initialize(
                host: settings.Host,
                port: settings.Port,
                retryTime: settings.RetryTime,
                messageType: MessageTypePositions);
        }

        private void InitializeRealtimeSender()
        {
            var settings = config
                .GetSection(nameof(Settings.RealtimeSender))
                .Get<Settings.RealtimeSender>();

            realtimeSender.Initialize(
                division: settings.Division,
                endpoint: settings.Endpoint,
                retryTime: settings.RetryTime,
                sessionStart: ebuefSessionStart);
        }

        private async void OnMessageReceived(object sender, MessageReceivedArgs e)
        {
            logger.LogDebug(
                "Positions-Nachricht empfangen: {content}",
                e.Content);

            try
            {
                var message = JsonConvert.DeserializeObject<RealTimeMessage>(e.Content);

                if (!string.IsNullOrWhiteSpace(message?.Zugnummer))
                {
                    var trainLeg = converter.Convert(message);

                    if (trainLeg != default)
                    {
                        realtimeSender.AddRealtime(trainLeg);
                        await databaseConnector.AddRealtimeAsync(trainLeg);
                    }
                    else
                    {
                        logger.LogDebug(
                            "Zur Positions-Nachricht konnte in der aktuellen Sitzung keine Fahrt gefunden werden.");
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

        private async void OnSessionStart(object sender, EventArgs e)
        {
            logger.LogDebug(
                "Nachricht zum Setzen der Fahrzeug-Grundaufstellung empfangen.");

            var allocations = await databaseConnector.GetVehicleAllocationsAsync();

            realtimeSender.AddAllocations(allocations);
        }

        #endregion Private Methods
    }
}

#pragma warning disable CA1031 // Do not catch general exception types