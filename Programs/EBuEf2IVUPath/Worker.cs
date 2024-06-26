using Commons.Enums;
using Commons.Extensions;
using Commons.Interfaces;
using Commons.Models;
using Commons.Settings;
using EBuEf2IVUBase;
using EnumerableExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EBuEf2IVUPath
{
    public class Worker
        : WorkerBase
    {
        #region Private Fields

        private const string MessageTypePaths = "Zugtrassen";

        private readonly IMessage2TrainRunConverter messageConverter;
        private readonly IMQTTReceiver mqttReceiver;
        private readonly IMulticastReceiver multicastReceiver;
        private readonly ITrainPathSender trainPathSender;

        private bool initalPathsSent;
        private bool isSessionRunning;

        #endregion Private Fields

        #region Public Constructors

        public Worker(IConfiguration config, IStateHandler sessionStateHandler, IDatabaseConnector databaseConnector,
            IMulticastReceiver multicastReceiver, IMQTTReceiver mqttReceiver, IMessage2TrainRunConverter messageConverter,
            ITrainPathSender trainPathSender, ILogger<Worker> logger)
            : base(config: config, sessionStateHandler: sessionStateHandler, databaseConnector: databaseConnector,
                  logger: logger, assembly: Assembly.GetExecutingAssembly())
        {
            this.multicastReceiver = multicastReceiver;
            this.multicastReceiver.MessageReceivedEvent += OnMessageReceived;

            this.mqttReceiver = mqttReceiver;
            this.mqttReceiver.MessageReceivedEvent += OnMessageReceived;

            this.messageConverter = messageConverter;
            this.trainPathSender = trainPathSender;
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override async Task ExecuteAsync(CancellationToken workerCancellationToken)
        {
            _ = InitializeConnectionAsync(workerCancellationToken);

            var pathReceiver = GetPathReceiver();

            await InitializePathSenderAsync();

            while (!workerCancellationToken.IsCancellationRequested)
            {
                _ = HandleSessionStateAsync(sessionStateHandler.StateType);

                var sessionCancellationToken = GetSessionCancellationToken(workerCancellationToken);

                while (!sessionCancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (isSessionRunning
                            && sessionStateHandler.StateType != StateType.IsPaused)
                        {
                            var receiverTask = pathReceiver.ExecuteAsync(sessionCancellationToken);

                            var senderTask = trainPathSender.ExecuteAsync(
                                ivuDatum: ebuefSession.IVUDatum,
                                sessionKey: ebuefSession.SessionKey,
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
                initalPathsSent = false;

                logger.LogInformation(
                    "EBuEf2IVUPath wird gestoppt.");
            }
        }

        protected override async Task HandleSessionStateAsync(StateType stateType)
        {
            if (stateType == StateType.IsEnded || stateType == StateType.IsPaused)
            {
                isSessionRunning = false;

                if (stateType == StateType.IsEnded)
                {
                    initalPathsSent = false;
                }
            }
            else if (stateType == StateType.InPreparation || stateType == StateType.IsRunning)
            {
                await InitializeSessionAsync();
                isSessionRunning = stateType == StateType.IsRunning;

                if (stateType == StateType.InPreparation || !initalPathsSent)
                {
                    await SendInitialPathesAsync();
                    initalPathsSent = true;
                }
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private async Task<IEnumerable<string>> GetLocationShortnamesAsync(Commons.Settings.TrainPathSender senderSettings)
        {
            var locationTypes = senderSettings.LocationTypes?.Split(
                separator: Commons.Settings.TrainPathSender.SettingsSeparator,
                options: StringSplitOptions.RemoveEmptyEntries);

            var result = await databaseConnector.GetLocationShortnamesAsync(locationTypes);

            if (result.AnyItem())
            {
                logger.LogDebug(
                    "Es werden nur die folgenden Netzpunkte als Trassenpunkte importiert: {locations}",
                    result.Merge());
            }

            return result;
        }

        private IMessageReceiver GetPathReceiver()
        {
            logger.LogInformation(
                "Der Nachrichten-Empfänger von EBuEf2IVUPath wird gestartet.");

            var settings = config
                .GetSection(nameof(TrainPathReceiver))
                .Get<TrainPathReceiver>();

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
                    messageType: MessageTypePaths);

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
                    messageType: MessageTypePaths);

                return mqttReceiver;
            }
        }

        private async Task InitializePathSenderAsync()
        {
            logger.LogInformation(
                "Der Trassen-Sender von EBuEf2IVUPath wird gestartet.");

            var settings = config
                .GetSection(nameof(Commons.Settings.TrainPathSender))
                .Get<Commons.Settings.TrainPathSender>();

            var ignoreTrainTypes = settings.IgnoreTrainTypes?.Split(
                separator: Commons.Settings.TrainPathSender.SettingsSeparator,
                options: StringSplitOptions.RemoveEmptyEntries);

            var locationShortnames = await GetLocationShortnamesAsync(settings);

            var host = settings.GetIVUAppServerHost();
            var port = settings.GetIVUAppServerPort() ?? 0;
            var isHttps = settings.GetIVUAppServerSecure() ?? false;

            trainPathSender.Initialize(
                host: host,
                port: port,
                isHttps: isHttps,
                username: settings.Username,
                password: settings.Password,
                path: settings.Path,
                retryTime: settings.RetryTime,
                infrastructureManager: settings.InfrastructureManager,
                orderingTransportationCompany: settings.OrderingTransportationCompany,
                stoppingReasonStop: settings.StoppingReasonStop,
                stoppingReasonPass: settings.StoppingReasonPass,
                trainPathStateRun: settings.TrainPathStateRun,
                trainPathStateCancelled: settings.TrainPathStateCancelled,
                importProfile: settings.ImportProfile,
                ignoreTrainTypes: ignoreTrainTypes,
                locationShortnames: locationShortnames,
                logRequests: settings.LogRequests);
        }

        private void OnMessageReceived(object sender, Commons.EventsArgs.MessageReceivedArgs e)
        {
            logger.LogDebug(
                "Zugtrassen-Nachricht empfangen: {content}",
                e.Content);

            try
            {
                var messages = JsonConvert.DeserializeObject<TrainPathMessage[]>(e.Content);
                var trainRuns = messageConverter.Convert(messages);

                if (trainRuns.AnyItem())
                {
                    trainPathSender.Add(trainRuns);
                }
                else
                {
                    logger.LogDebug(
                        "Die Zugtrassen-Nachricht hat keine Fahrten enthalten.");
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
                    "Die Nachricht kann nicht in eine Zugtrasse umgeformt werden: {message}",
                    serializationException.Message);
            }
        }

        private async Task SendInitialPathesAsync()
        {
            logger.LogInformation(
                "Die initialen Zugtrassen werden importiert.");

            if (ebuefSession == default)
            {
                logger.LogWarning(
                    "Das initiale Sitzungsupdate wurde bisher nicht empfangen. " +
                    "Daher können keine Zugtrassen importiert werden.");
            }
            else
            {
                var senderSettings = config
                    .GetSection(nameof(Commons.Settings.TrainPathSender))
                    .Get<Commons.Settings.TrainPathSender>();

                var trainRuns = await databaseConnector.GetTrainRunsPlanAsync(
                    timetableId: ebuefSession.FahrplanId,
                    weekday: ebuefSession.Wochentag,
                    preferPrognosis: senderSettings.PreferPrognosis);

                if (trainRuns.AnyItem())
                {
                    trainPathSender.Add(trainRuns);
                }
                else
                {
                    logger.LogDebug(
                        "In der Datenbank wurden keine Zugtrassen gefunden.");
                }
            }
        }

        #endregion Private Methods
    }
}