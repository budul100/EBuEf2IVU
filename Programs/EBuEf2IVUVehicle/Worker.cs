using Commons.Enums;
using Commons.EventsArgs;
using Commons.Extensions;
using Commons.Interfaces;
using Commons.Models;
using Commons.Settings;
using EBuEf2IVUBase;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EBuEf2IVUVehicle
{
    public class Worker
        : WorkerBase
    {
        #region Private Fields

        private const string MessageTypePositions = "Echtzeit-Positionen";

        private readonly IMessage2LegConverter messageConverter;
        private readonly IMQTTReceiver mqttReceiver;
        private readonly IMulticastReceiver multicastReceiver;
        private readonly IRealtimeSender realtimeSender;
        private readonly IRealtimeSenderIS realtimeSenderIS;

        private bool ignorePrognosis;
        private bool initalAllocationsSent;
        private bool isSessionRunning;
        private bool useInterfaceServer;

        #endregion Private Fields

        #region Public Constructors

        public Worker(IConfiguration config, IStateHandler sessionStateHandler, IDatabaseConnector databaseConnector,
            IMulticastReceiver multicastReceiver, IMQTTReceiver mqttReceiver, IMessage2LegConverter messageConverter,
            IRealtimeSender realtimeSender, IRealtimeSenderIS realtimeSenderIS, ILogger<Worker> logger)
            : base(config: config, sessionStateHandler: sessionStateHandler, databaseConnector: databaseConnector,
                  logger: logger, assembly: Assembly.GetExecutingAssembly())
        {
            this.multicastReceiver = multicastReceiver;
            this.multicastReceiver.MessageReceivedEvent += OnMessageReceivedAsync;

            this.mqttReceiver = mqttReceiver;
            this.mqttReceiver.MessageReceivedEvent += OnMessageReceivedAsync;

            this.messageConverter = messageConverter;
            this.realtimeSenderIS = realtimeSenderIS;
            this.realtimeSender = realtimeSender;
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override async Task ExecuteAsync(CancellationToken workerCancellationToken)
        {
            _ = InitializeConnectionAsync(workerCancellationToken);

            var positionReceiver = GetPositionReceiver();

            InitializeRealtimeSender();

            while (!workerCancellationToken.IsCancellationRequested)
            {
                _ = HandleSessionStateAsync(sessionStateHandler.StateType);

                var sessionCancellationToken = GetSessionCancellationToken(workerCancellationToken);

                var receiverTask = positionReceiver.ExecuteAsync(sessionCancellationToken);

                while (!sessionCancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (isSessionRunning
                            && sessionStateHandler.StateType != StateType.IsPaused)
                        {
                            var senderTask = useInterfaceServer
                                ? realtimeSenderIS.ExecuteAsync(
                                    ivuDatum: ebuefSession.IVUDatum,
                                    sessionStart: ebuefSession.SessionStart,
                                    cancellationToken: sessionCancellationToken)
                                : realtimeSender.ExecuteAsync(
                                    ivuDatum: ebuefSession.IVUDatum,
                                    sessionStart: ebuefSession.SessionStart,
                                    cancellationToken: sessionCancellationToken);

                            await Task.WhenAny(
                                receiverTask,
                                senderTask);
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

                isSessionRunning = false;
                initalAllocationsSent = false;

                logger.LogInformation(
                    "EBuEf2IVUVehicle wird gestoppt.");
            }
        }

        protected override async Task HandleSessionStateAsync(StateType stateType)
        {
            if (stateType == StateType.IsEnded || stateType == StateType.IsPaused)
            {
                isSessionRunning = false;

                if (stateType == StateType.IsEnded)
                {
                    initalAllocationsSent = false;
                }
            }
            else if (stateType == StateType.IsRunning)
            {
                await InitializeSessionAsync();
                isSessionRunning = stateType == StateType.IsRunning;

                if (!initalAllocationsSent)
                {
                    await SendInitialAllocationsAsync();
                    initalAllocationsSent = true;
                }
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private IMessageReceiver GetPositionReceiver()
        {
            logger.LogInformation(
                "Der Nachrichten-Empfänger von EBuEf2IVUVehicle wird gestartet.");

            var settings = config
                .GetSection(nameof(PositionsReceiver))
                .Get<PositionsReceiver>();

            var useMulticast = settings.GetEBuEfUseMC();

            if (useMulticast)
            {
                var host = settings.GetEBuEfHostMC();
                var port = settings.GetEBuEfPort()
                    ?? ConnectorEBuEfBase.MulticastPort;

                multicastReceiver.Initialize(
                    host: host,
                    port: port,
                    retryTime: settings.RetryTime,
                    messageType: MessageTypePositions);

                return multicastReceiver;
            }
            else
            {
                var host = settings.GetEBuEfHostMQTT();
                var port = settings.GetEBuEfPort();

                mqttReceiver.Initialize(
                    server: host,
                    port: port,
                    topic: settings.Topic,
                    retryTime: settings.RetryTime,
                    messageType: MessageTypePositions);

                return mqttReceiver;
            }
        }

        private void InitializeRealtimeSender()
        {
            logger.LogInformation(
                "Der Ist-Daten-Sender von EBuEf2IVUVehicle wird gestartet.");

            var settings = config
                .GetSection(nameof(Commons.Settings.RealtimeSender))
                .Get<Commons.Settings.RealtimeSender>();

            ignorePrognosis = settings.IgnorePrognosis;
            useInterfaceServer = settings.UseInterfaceServer;

            if (useInterfaceServer)
            {
                var endpoint = settings.GetIVUIFServerEndpoint();

                realtimeSenderIS.Initialize(
                    endpoint: endpoint,
                    division: settings.Division,
                    retryTime: settings.RetryTime);
            }
            else
            {
                var host = settings.GetIVUAppServerHost();
                var port = settings.GetIVUAppServerPort() ?? 0;
                var isHttps = settings.GetIVUAppServerSecure() ?? false;

                realtimeSender.Initialize(
                    host: host,
                    port: port,
                    isHttps: isHttps,
                    username: settings.Username,
                    password: settings.Password,
                    path: settings.Path,
                    division: settings.Division,
                    retryTime: settings.RetryTime);
            }
        }

        private async void OnMessageReceivedAsync(object sender, MessageReceivedArgs e)
        {
            logger.LogDebug(
                "Positions-Nachricht empfangen: {content}",
                e.Content);

            try
            {
                var message = JsonConvert.DeserializeObject<RealTimeMessage>(e.Content);

                if (string.IsNullOrWhiteSpace(message?.Zugnummer))
                {
                    logger.LogDebug("In der Nachricht ist keine Zugnummer enthalten und wird daher verworfen.");
                }
                else if (ignorePrognosis
                    && message.Modus == MessageType.Prognose)
                {
                    logger.LogDebug("In der Nachricht enthält eine Prognose, diese wird laut aktueller Einstellung verworfen.");
                }
                else
                {
                    var trainLeg = messageConverter.Convert(message);

                    if (trainLeg != default)
                    {
                        if (useInterfaceServer)
                        {
                            realtimeSenderIS.Add(trainLeg);
                        }
                        else
                        {
                            realtimeSender.Add(trainLeg);
                        }

                        if (!trainLeg.IstPrognose)
                        {
                            await databaseConnector.AddRealtimeAsync(trainLeg);
                        }
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
                    readerException,
                    "Die Nachricht kann nicht gelesen werden: {message}",
                    readerException.Message);
            }
            catch (JsonSerializationException serializationException)
            {
                logger.LogError(
                    serializationException,
                    "Die Nachricht kann nicht in eine Echtzeitmeldung umgeformt werden: {message}",
                    serializationException.Message);
            }
        }

        private async Task SendInitialAllocationsAsync()
        {
            logger.LogDebug(
                "Die initiale Fahrzeug-Grundaufstellung wird gesendet.");

            var allocations = await databaseConnector.GetVehicleAllocationsAsync();

            if (useInterfaceServer)
            {
                realtimeSenderIS.Add(allocations);
            }
            else
            {
                realtimeSender.Add(allocations);
            }
        }

        #endregion Private Methods
    }
}