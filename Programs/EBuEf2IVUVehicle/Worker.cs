using Commons.Enums;
using Commons.EventsArgs;
using Commons.Extensions;
using Commons.Interfaces;
using Commons.Models;
using Commons.Settings;
using EBuEf2IVUBase;
using EnumerableExtensions;
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

        private static readonly object initializationLock = new();

        private readonly IMessage2LegConverter messageConverter;
        private readonly IMQTTReceiver mqttReceiver;
        private readonly IMulticastReceiver multicastReceiver;
        private readonly IRealtimeSender realtimeSender;
        private readonly IRealtimeSenderIS realtimeSenderIS;

        private bool ignorePrognosis;
        private bool initalAllocationsSent;
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
                _ = HandleSessionStateAsync(
                    stateType: sessionStateHandler.StateType);

                var sessionCancellationToken = GetSessionCancellationToken(
                    workerCancellationToken: workerCancellationToken);

                var receiverTask = positionReceiver.ExecuteAsync(
                    cancellationToken: sessionCancellationToken);
                var senderTask = default(Task);

                while (!sessionCancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (ebuefSession != default
                            && ((sessionStateHandler?.StateType == StateType.InPreparation)
                            || (sessionStateHandler?.StateType == StateType.IsRunning)))
                        {
                            if (useInterfaceServer)
                            {
                                senderTask ??= realtimeSenderIS.ExecuteAsync(
                                    ivuDatum: ebuefSession.IVUDatum,
                                    sessionStart: ebuefSession.SessionStart,
                                    cancellationToken: sessionCancellationToken);
                            }
                            else
                            {
                                senderTask ??= realtimeSender.ExecuteAsync(
                                    ivuDatum: ebuefSession.IVUDatum,
                                    sessionStart: ebuefSession.SessionStart,
                                    cancellationToken: sessionCancellationToken);
                            }

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

                initalAllocationsSent = false;

                logger.LogInformation(
                    "EBuEf2IVUVehicle wird gestoppt.");
            }
        }

        protected override async Task HandleSessionStateAsync(StateType stateType)
        {
            if (stateType == StateType.IsEnded)
            {
                initalAllocationsSent = false;
            }
            else if (stateType == StateType.InPreparation || stateType == StateType.IsRunning)
            {
                await InitializeSessionAsync();

                lock (initializationLock)
                {
                    SendInitialAllocations();
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
                    var trainLeg = messageConverter.Convert(
                        message: message);

                    if (trainLeg != default)
                    {
                        if (useInterfaceServer)
                        {
                            realtimeSenderIS.Add(
                                trainLeg: trainLeg);
                        }
                        else
                        {
                            realtimeSender.Add(
                                trainLeg: trainLeg);
                        }

                        if (!trainLeg.IstPrognose)
                        {
                            await databaseConnector.AddRealtimeAsync(
                                leg: trainLeg);
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

        private void SendInitialAllocations()
        {
            if (ebuefSession == default)
            {
                logger.LogWarning(
                    "Das initiale Sitzungsupdate wurde bisher nicht empfangen. " +
                    "Daher kann keine Fahrzeug-Grundaufstellung gesendet werden.");
            }
            else if (!initalAllocationsSent)
            {
                logger.LogDebug(
                    "Die initiale Fahrzeug-Grundaufstellung wird gesendet.");

                var allocations = Task.Run(databaseConnector.GetVehicleAllocationsAsync).Result;

                if (allocations.AnyItem())
                {
                    if (useInterfaceServer)
                    {
                        realtimeSenderIS.Add(allocations);
                    }
                    else
                    {
                        realtimeSender.Add(allocations);
                    }

                    initalAllocationsSent = true;
                }
                else
                {
                    logger.LogInformation(
                        "In der Grundaufstellung sind keine Fahrzeuge eingetragen.");
                }
            }
        }
    }

    #endregion Private Methods
}