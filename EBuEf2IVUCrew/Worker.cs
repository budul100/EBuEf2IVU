using Common.Enums;
using Common.EventsArgs;
using Common.Interfaces;
using EBuEf2IVUCrew.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EBuEf2IVUCrew
{
    internal class Worker
        : BackgroundService
    {
        #region Private Fields

        private readonly IConfiguration config;
        private readonly IConnector databaseConnector;
        private readonly ILogger logger;
        private readonly IStateHandler sessionStateHandler;

        private TimeSpan queryDurationFuture;
        private TimeSpan queryDurationPast;
        private CancellationTokenSource sessionCancellationTokenSource;
        private TimeSpan timeshift;
        private TrainRunQueries trainRunQuerySettings;

        #endregion Private Fields

        #region Public Constructors

        public Worker(IConfiguration config, ILogger<Worker> logger, IStateHandler sessionStateHandler,
            IConnector databaseConnector)
        {
            this.config = config;
            this.logger = logger;

            this.databaseConnector = databaseConnector;

            this.sessionStateHandler = sessionStateHandler;
            this.sessionStateHandler.SessionStartedEvent += OnSessionStartedAsync;
            this.sessionStateHandler.SessionChangedEvent += OnSessionChangedAsync;
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override async Task ExecuteAsync(CancellationToken workerCancellationToken)
        {
            while (!workerCancellationToken.IsCancellationRequested)
            {
                var sessionCancellationToken = GetSessionCancellationToken(workerCancellationToken);

                InitializeConnector(sessionCancellationToken);
                InitializeQueries();
                InitializeStateHandler();

                trainRunQuerySettings = config
                    .GetSection(nameof(TrainRunQueries))
                    .Get<TrainRunQueries>();

                await TestAsync();
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private CancellationToken GetSessionCancellationToken(CancellationToken workerCancellationToken)
        {
            sessionCancellationTokenSource = new CancellationTokenSource();
            workerCancellationToken.Register(() => sessionCancellationTokenSource.Cancel());

            return sessionCancellationTokenSource.Token;
        }

        private TimeSpan GetSimTime()
        {
            return DateTime.Now.Add(timeshift).TimeOfDay;
        }

        private void InitializeConnector(CancellationToken sessionCancellationToken)
        {
            var settings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            databaseConnector.Initialize(
                connectionString: settings.ConnectionString,
                retryTime: settings.RetryTime,
                cancellationToken: sessionCancellationToken);
        }

        private void InitializeQueries()
        {
            var settings = config
                .GetSection(nameof(TrainRunQueries))
                .Get<TrainRunQueries>();

            queryDurationPast = new TimeSpan(
                hours: 0,
                minutes: settings.AbfrageInVergangenheit * -1,
                seconds: 0);

            queryDurationFuture = new TimeSpan(
                hours: 0,
                minutes: settings.AbfrageInZukunft,
                seconds: 0);
        }

        private void InitializeStateHandler()
        {
            var settings = config
                .GetSection(nameof(StatusReceiver))
                .Get<StatusReceiver>();

            sessionStateHandler.Initialize(
                ipAdress: settings.IpAddress,
                port: settings.ListenerPort,
                retryTime: settings.RetryTime,
                startPattern: settings.StartPattern,
                statusPattern: settings.StatusPattern);
        }

        private async void OnSessionChangedAsync(object sender, StateChangedArgs e)
        {
            if (e.State == SessionStates.IsRunning)
            {
                logger?.LogInformation("Sessionstart-Nachricht empfangen.");

                await StartIVUSessionAsync();
            }
            else if (e.State == SessionStates.IsPaused)
            {
                logger.LogInformation("Sessionpause-Nachricht empfangen. Die Nachrichtenempfänger, " +
                    "Datenbank-Verbindungen und IVU-Sender werden zurückgesetzt.");

                sessionCancellationTokenSource.Cancel();
            }
        }

        private async void OnSessionStartedAsync(object sender, EventArgs e)
        {
            logger?.LogInformation("Sessionstart-Nachricht empfangen.");

            await StartIVUSessionAsync();
        }

        private async Task StartIVUSessionAsync()
        {
            var currentSession = await databaseConnector.GetEBuEfSessionAsync();

            timeshift = currentSession.Verschiebung;
        }

        private async Task TestAsync()
        {
            var minTime = GetSimTime().Add(queryDurationPast);
            var maxTime = GetSimTime().Add(queryDurationFuture);

            var stops = await databaseConnector.GetTrainRunsAsync(
                minTime: minTime,
                maxTime: maxTime);

            foreach (var stop in stops)
            {
                logger.LogDebug(stop.ToString());
            }
        }

        #endregion Private Methods
    }
}