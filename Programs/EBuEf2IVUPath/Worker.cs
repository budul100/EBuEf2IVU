using Common.Enums;
using Common.Interfaces;
using Common.Models;
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
        private readonly IMessageReceiver trainPathReceiver;
        private readonly ITrainPathSender trainPathSender;

        private bool initalPathsSent;
        private bool isSessionInitialized;

        #endregion Private Fields

        #region Public Constructors

        public Worker(IConfiguration config, IStateHandler sessionStateHandler, IDatabaseConnector databaseConnector,
            IMessageReceiver messageReceiver, IMessage2TrainRunConverter messageConverter, ITrainPathSender trainPathSender,
            ILogger<Worker> logger)
            : base(config: config, sessionStateHandler: sessionStateHandler, databaseConnector: databaseConnector,
                  logger: logger, assembly: Assembly.GetExecutingAssembly())
        {
            this.trainPathReceiver = messageReceiver;
            this.trainPathReceiver.MessageReceivedEvent += OnMessageReceived;

            this.messageConverter = messageConverter;
            this.trainPathSender = trainPathSender;
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override async Task ExecuteAsync(CancellationToken workerCancellationToken)
        {
            _ = InitializeConnectionAsync(workerCancellationToken);

            InitializePathReceiver();
            await InitializePathSenderAsync();

            while (!workerCancellationToken.IsCancellationRequested)
            {
                var sessionCancellationToken = GetSessionCancellationToken(workerCancellationToken);

                _ = HandleSessionStateAsync(sessionStateHandler.StateType);

                while (!sessionCancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (isSessionInitialized
                            && sessionStateHandler.StateType != StateType.IsPaused)
                        {
                            await Task.WhenAny(
                                trainPathReceiver.ExecuteAsync(sessionCancellationToken),
                                trainPathSender.ExecuteAsync(sessionCancellationToken));
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

                initalPathsSent = false;
                isSessionInitialized = false;

                logger.LogInformation(
                    "EBuEf2IVUPath wird gestoppt.");
            }
        }

        protected override async Task HandleSessionStateAsync(StateType stateType)
        {
            if (stateType == StateType.IsEnded
                || stateType == StateType.IsPaused)
            {
                isSessionInitialized = false;
            }
            else if ((stateType == StateType.InPreparation || stateType == StateType.IsRunning)
                && !isSessionInitialized)
            {
                await InitializeSessionAsync();

                isSessionInitialized = true;

                if (!initalPathsSent)
                {
                    await SendInitialPathesAsync();
                    initalPathsSent = true;
                }
            }
        }

        protected override async Task InitializeSessionAsync()
        {
            await base.InitializeSessionAsync();

            messageConverter.Initialize(
                ivuDatum: ebuefSession?.IVUDatum,
                sessionKey: ebuefSession?.SessionKey);
        }

        #endregion Protected Methods

        #region Private Methods

        private async Task<IEnumerable<string>> GetLocationShortnamesAsync(Common.Settings.TrainPathSender senderSettings)
        {
            var locationTypes = senderSettings.LocationTypes?.Split(
                separator: Common.Settings.TrainPathSender.SettingsSeparator,
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

        private void InitializePathReceiver()
        {
            logger.LogInformation(
                "Der Nachrichten-Empfänger von EBuEf2IVUPath wird gestartet.");

            var receiverSettings = config
                .GetSection(nameof(Common.Settings.TrainPathReceiver))
                .Get<Common.Settings.TrainPathReceiver>();

            trainPathReceiver.Initialize(
                host: receiverSettings.Host,
                port: receiverSettings.Port,
                retryTime: receiverSettings.RetryTime,
                messageType: MessageTypePaths);
        }

        private async Task InitializePathSenderAsync()
        {
            logger.LogInformation(
                "Der Trassen-Sender von EBuEf2IVUPath wird gestartet.");

            var senderSettings = config
                .GetSection(nameof(Common.Settings.TrainPathSender))
                .Get<Common.Settings.TrainPathSender>();

            var ignoreTrainTypes = senderSettings.IgnoreTrainTypes?.Split(
                separator: Common.Settings.TrainPathSender.SettingsSeparator,
                options: StringSplitOptions.RemoveEmptyEntries);

            var locationShortnames = await GetLocationShortnamesAsync(senderSettings);

            trainPathSender.Initialize(
                host: senderSettings.Host,
                port: senderSettings.Port,
                path: senderSettings.Path,
                username: senderSettings.Username,
                password: senderSettings.Password,
                isHttps: senderSettings.IsHttps,
                retryTime: senderSettings.RetryTime,
                infrastructureManager: senderSettings.InfrastructureManager,
                orderingTransportationCompany: senderSettings.OrderingTransportationCompany,
                stoppingReasonStop: senderSettings.StoppingReasonStop,
                stoppingReasonPass: senderSettings.StoppingReasonPass,
                trainPathStateRun: senderSettings.TrainPathStateRun,
                trainPathStateCancelled: senderSettings.TrainPathStateCancelled,
                importProfile: senderSettings.ImportProfile,
                ignoreTrainTypes: ignoreTrainTypes,
                locationShortnames: locationShortnames);
        }

        private void OnMessageReceived(object sender, Common.EventsArgs.MessageReceivedArgs e)
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
                    .GetSection(nameof(Common.Settings.TrainPathSender))
                    .Get<Common.Settings.TrainPathSender>();

                var trainRuns = await databaseConnector.GetTrainRunsPlanAsync(
                    timetableId: ebuefSession.FahrplanId,
                    weekday: ebuefSession.Wochentag,
                    ivuDatum: ebuefSession.IVUDatum,
                    sessionKey: ebuefSession.SessionKey,
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