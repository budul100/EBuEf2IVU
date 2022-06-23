using Common.Enums;
using Common.EventsArgs;
using Common.Interfaces;
using Common.Models;
using EBuEf2IVUBase;
using EBuEf2IVUVehicle.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;
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
        private readonly IRealtimeSenderIS realtimeSender20;
        private readonly IRealtimeSender realtimeSender21;
        private bool isSessionInitialized;
        private bool useInterfaceServer;

        #endregion Private Fields

        #region Public Constructors

        public Worker(IConfiguration config, IStateHandler sessionStateHandler, IMessageReceiver positionsReceiver,
            IDatabaseConnector databaseConnector, IRealtimeSenderIS realtimeSender20, IRealtimeSender realtimeSender21,
            ILogger<Worker> logger)
            : base(config, sessionStateHandler, databaseConnector, logger, Assembly.GetExecutingAssembly())
        {
            this.sessionStateHandler.SessionChangedEvent += OnSessionChangedAsync;

            this.positionsReceiver = positionsReceiver;
            this.positionsReceiver.MessageReceivedEvent += OnMessageReceived;

            this.realtimeSender20 = realtimeSender20;
            this.realtimeSender21 = realtimeSender21;

            converter = new Message2TrainLeg(
                config: config,
                logger: logger,
                ivuSessionDate: ivuSessionDate);
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override async Task ExecuteAsync(CancellationToken workerCancellationToken)
        {
            _ = InitializeConnectionAsync(workerCancellationToken);

            InitializePositionReceiver();
            InitializeRealtimeSender();

            while (!workerCancellationToken.IsCancellationRequested)
            {
                var sessionCancellationToken = GetSessionCancellationToken(workerCancellationToken);

                isSessionInitialized = false;

                while (!sessionCancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (isSessionInitialized
                            && sessionStateHandler.SessionStatus != SessionStatusType.IsPaused)
                        {
                            if (useInterfaceServer)
                            {
                                await Task.WhenAny(
                                    positionsReceiver.ExecuteAsync(sessionCancellationToken),
                                    realtimeSender20.ExecuteAsync(sessionCancellationToken));
                            }
                            else
                            {
                                await Task.WhenAny(
                                    positionsReceiver.ExecuteAsync(sessionCancellationToken),
                                    realtimeSender21.ExecuteAsync(sessionCancellationToken));
                            }
                        }
                        else
                        {
                            // Allow other tasks to run

                            await Task.Delay(
                                delay: delay,
                                cancellationToken: sessionCancellationToken);
                        }
                    }
                    catch (TaskCanceledException)
                    { }
                }

                logger.LogInformation(
                    "EBuEf2IVUVehicle wird gestoppt.");
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void InitializePositionReceiver()
        {
            logger.LogInformation(
                "Der Nachrichten-Empfänger von EBuEf2IVUVehicle wird gestartet.");

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
            logger.LogInformation(
                "Der Ist-Daten-Sender von EBuEf2IVUVehicle wird gestartet.");

            var settings = config
                .GetSection(nameof(Settings.RealtimeSender))
                .Get<Settings.RealtimeSender>();

            useInterfaceServer = settings.UseInterfaceServer;

            if (useInterfaceServer)
            {
                realtimeSender20.Initialize(
                    endpoint: settings.Endpoint,
                    division: settings.Division,
                    sessionStart: ebuefSessionStart,
                    retryTime: settings.RetryTime);
            }
            else
            {
                realtimeSender21.Initialize(
                    host: settings.Host,
                    port: settings.Port,
                    path: settings.Path,
                    username: settings.Username,
                    password: settings.Password,
                    isHttps: settings.IsHttps,
                    division: settings.Division,
                    sessionStart: ebuefSessionStart,
                    retryTime: settings.RetryTime);
            }
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
                        if (useInterfaceServer)
                        {
                            realtimeSender20.Add(trainLeg);
                        }
                        else
                        {
                            realtimeSender21.Add(trainLeg);
                        }

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

        private async void OnSessionChangedAsync(object sender, StateChangedArgs e)
        {
            if (!isSessionInitialized
                && e.State == SessionStatusType.IsRunning)
            {
                await InitializeSessionAsync();
                await SendInitialAllocationsAsync();

                isSessionInitialized = true;
            }
        }

        private async Task SendInitialAllocationsAsync()
        {
            logger.LogDebug(
                "Die initiale Fahrzeug-Grundaufstellung wird gesendet.");

            var allocations = await databaseConnector.GetVehicleAllocationsAsync();

            if (useInterfaceServer)
            {
                realtimeSender20.Add(allocations);
            }
            else
            {
                realtimeSender21.Add(allocations);
            }
        }

        #endregion Private Methods
    }
}