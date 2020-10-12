#pragma warning disable CA1031 // Do not catch general exception types

using Common.Enums;
using Common.Interfaces;
using EBuEf2IVUBase;
using EBuEf2IVUPath.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EBuEf2IVUPath
{
    internal class Worker
        : WorkerBase
    {
        #region Private Fields

        private readonly ITrainPathSender trainPathSender;

        private TimeSpan serviceInterval;

        #endregion Private Fields

        #region Public Constructors

        public Worker(IConfiguration config, IStateHandler sessionStateHandler, IDatabaseConnector databaseConnector,
            ITrainPathSender trainPathSender, ILogger<Worker> logger)
            : base(config, sessionStateHandler, databaseConnector, logger)
        {
            this.trainPathSender = trainPathSender;
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override async Task ExecuteAsync(CancellationToken workerCancellationToken)
        {
            InitializeStateHandler();
            sessionStateHandler.Run(workerCancellationToken);

            currentState = SessionStates.IsRunning;

            while (!workerCancellationToken.IsCancellationRequested)
            {
                logger.LogInformation(
                    "Die Nachrichtenempfänger, Datenbank-Verbindungen und IVU-Sender von EBuEf2IVUPath werden zurückgesetzt.");

                var sessionCancellationToken = GetSessionCancellationToken(workerCancellationToken);

                InitializeService();
                InitializeConnector(sessionCancellationToken);

                await StartIVUSessionAsync();

                while (!sessionCancellationToken.IsCancellationRequested)
                {
                    if (currentState == SessionStates.IsRunning)
                    {
                        //await CheckCrewsAsync(sessionCancellationToken);

                        try
                        {
                            await Task.Delay(
                                delay: serviceInterval,
                                cancellationToken: sessionCancellationToken);
                        }
                        catch (TaskCanceledException)
                        {
                            logger.LogInformation(
                                "EBuEf2IVUPath wird beendet.");
                        }
                    }
                }
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void InitializeSender()
        {
            var senderSettings = config
                .GetSection(nameof(TrainPathSender))
                .Get<TrainPathSender>();

            trainPathSender.Initialize(senderSettings.ho)
        }

        private void InitializeService()
        {
            var serviceSettings = config
                .GetSection(nameof(ServiceSettings))
                .Get<ServiceSettings>();

            serviceInterval = new TimeSpan(
                hours: 0,
                minutes: 0,
                seconds: serviceSettings.AbfrageIntervalSek);
        }

        #endregion Private Methods
    }
}

#pragma warning disable CA1031 // Do not catch general exception types