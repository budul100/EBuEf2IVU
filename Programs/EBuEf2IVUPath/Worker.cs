#pragma warning disable CA1031 // Do not catch general exception types

using Common.Interfaces;
using Common.Models;
using EBuEf2IVUBase;
using EnumerableExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EBuEf2IVUPath
{
    internal class Worker
        : WorkerBase
    {
        #region Private Fields

        private const string MessageTypePaths = "Zugtrassen";

        private readonly IMessageReceiver trainPathReceiver;
        private readonly ITrainPathSender trainPathSender;

        private bool preferPrognosis;

        #endregion Private Fields

        #region Public Constructors

        public Worker(IConfiguration config, IStateHandler sessionStateHandler, IMessageReceiver trainPathReceiver,
            IDatabaseConnector databaseConnector, ITrainPathSender trainPathSender, ILogger logger)
            : base(config, sessionStateHandler, databaseConnector, logger)
        {
            this.sessionStateHandler.SessionStartedEvent += OnSessionStart;

            this.trainPathReceiver = trainPathReceiver;
            this.trainPathReceiver.MessageReceivedEvent += OnMessageReceived;

            this.trainPathSender = trainPathSender;
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
                    "Die Nachrichtenempfänger, Datenbank-Verbindungen und IVU-Sender von EBuEf2IVUPath werden zurückgesetzt.");

                var sessionCancellationToken = GetSessionCancellationToken(workerCancellationToken);

                InitializePathReceiver();
                InitializePathSender();

                InitializeDatabaseConnector(sessionCancellationToken);

                await StartIVUSessionAsync();

                while (!sessionCancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.WhenAny(
                            trainPathReceiver.RunAsync(sessionCancellationToken),
                            trainPathSender.RunAsnc(sessionCancellationToken));
                    }
                    catch (TaskCanceledException)
                    {
                        logger.LogInformation(
                            "EBuEf2IVUPath wird beendet.");
                    }
                };
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void InitializePathReceiver()
        {
            var receiverSettings = config
                .GetSection(nameof(Settings.TrainPathReceiver))
                .Get<Settings.TrainPathReceiver>();

            trainPathReceiver.Initialize(
                host: receiverSettings.Host,
                port: receiverSettings.Port,
                retryTime: receiverSettings.RetryTime,
                messageType: MessageTypePaths);
        }

        private void InitializePathSender()
        {
            var senderSettings = config
                .GetSection(nameof(Settings.TrainPathSender))
                .Get<Settings.TrainPathSender>();

            preferPrognosis = senderSettings.PreferPrognosis;

            trainPathSender.Initialize(
                host: senderSettings.Host,
                port: senderSettings.Port,
                path: senderSettings.Path,
                username: senderSettings.Username,
                password: senderSettings.Password,
                isHttps: senderSettings.IsHttps,
                retryTime: senderSettings.RetryTime,
                sessionDate: ivuSessionDate,
                infrastructureManager: senderSettings.InfrastructureManager,
                orderingTransportationCompany: senderSettings.OrderingTransportationCompany,
                trainPathState: senderSettings.TrainPathState,
                stoppingReasonStop: senderSettings.StoppingReasonStop,
                stoppingReasonPass: senderSettings.StoppingReasonPass,
                importProfile: senderSettings.ImportProfile);
        }

        private async void OnMessageReceived(object sender, Common.EventsArgs.MessageReceivedArgs e)
        {
            logger.LogDebug(
                "Zugtrassen-Nachricht empfangen: {content}",
                e.Content);

            try
            {
                var message = JsonConvert.DeserializeObject<TrainPathMessage>(e.Content);

                if (!string.IsNullOrWhiteSpace(message?.TrainId))
                {
                    var trainRuns = await databaseConnector.GetTrainRunsAsync(
                        trainId: message.TrainId,
                        preferPrognosis: preferPrognosis);

                    if (trainRuns.AnyItem())
                    {
                        trainPathSender.AddTrains(trainRuns);
                    }
                    else
                    {
                        logger.LogDebug(
                            "Zur Zugtrassen-Nachricht konnte in der aktuellen Sitzung keine Fahrt gefunden werden.");
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
                    "Die Nachricht kann nicht in eine Zugtrasse umgeformt werden: {message}",
                    serializationException.Message);
            }
        }

        private async void OnSessionStart(object sender, EventArgs e)
        {
            logger.LogDebug(
                "Nachricht zum Import aller Zugtrassen empfangen.");

            var trainRuns = await databaseConnector.GetTrainRunsAsync(preferPrognosis);

            trainPathSender.AddTrains(trainRuns);
        }

        #endregion Private Methods
    }
}

#pragma warning disable CA1031 // Do not catch general exception types