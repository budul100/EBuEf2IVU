using Common.Enums;
using Common.EventsArgs;
using Common.Interfaces;
using EBuEf2IVUCrew.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EBuEf2IVUCrew
{
    internal class Worker
        : BackgroundService
    {
        #region Private Fields

        private readonly IConfiguration config;
        private readonly ICrewChecker crewChecker;
        private readonly IDatabaseConnector databaseConnector;
        private readonly ILogger logger;
        private readonly IStateHandler sessionStateHandler;

        private IVUCrewChecker checkerSettings;
        private DateTime ivuSessionDate = DateTime.Now;
        private TimeSpan queryDurationFuture;
        private TimeSpan queryDurationPast;
        private TimeSpan serviceInterval;
        private CancellationTokenSource sessionCancellationTokenSource;
        private TimeSpan timeshift;

        #endregion Private Fields

        #region Public Constructors

        public Worker(IConfiguration config, ILogger<Worker> logger, IStateHandler sessionStateHandler,
            IDatabaseConnector databaseConnector, ICrewChecker crewChecker)
        {
            this.config = config;
            this.logger = logger;

            var assemblyInfo = Assembly.GetExecutingAssembly().GetName();
            logger.LogInformation($"{assemblyInfo.Name} (Version {assemblyInfo.Version.Major}.{assemblyInfo.Version.Minor}) wird gestartet.");

            this.databaseConnector = databaseConnector;
            this.crewChecker = crewChecker;

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

                InitializeService();
                InitializeStateHandler();
                InitializeConnector(sessionCancellationToken);
                InitializeChecker(sessionCancellationToken);

                await StartIVUSessionAsync();

                while (!sessionCancellationToken.IsCancellationRequested)
                {
                    await Task.WhenAny(
                        sessionStateHandler.RunAsync(sessionCancellationToken),
                        CheckCrewsAsync(sessionCancellationToken));

                    await Task.Delay(
                        delay: serviceInterval,
                        cancellationToken: sessionCancellationToken);
                }
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private async Task CheckCrewsAsync(CancellationToken sessionCancellationToken)
        {
            var minTime = GetSimTime()
                .Add(queryDurationPast).TimeOfDay;
            var maxTime = GetSimTime()
                .Add(queryDurationFuture).TimeOfDay;

            var trainRuns = await databaseConnector.GetTrainRunsAsync(
                minTime: minTime,
                maxTime: maxTime);

            logger.LogDebug(@$"In der EBuEf-DB wurden {trainRuns.Count()} Züge für den " +
                @$"Zeitraum zwischen {minTime:hh\:mm} und {maxTime:hh\:mm} gefunden.");

            if (trainRuns.Any())
            {
                var tripNumbers = trainRuns
                    .Select(t => t.Zugnummer).ToArray();
                var crewingElements = await crewChecker.GetCrewingElementsAsync(
                    tripNumbers: tripNumbers,
                    date: ivuSessionDate,
                    cancellationToken: sessionCancellationToken);

                logger.LogDebug($"In der IVU.rail wurden {crewingElements.Count()} Besatzungseinträge zu den Zügen gefunden.");
                logger.LogDebug(string.Join(@"\r\n\", crewingElements));

                if (crewingElements.Any())
                {
                    await databaseConnector.SetCrewingsAsync(crewingElements);
                }
            }
        }

        private CancellationToken GetSessionCancellationToken(CancellationToken workerCancellationToken)
        {
            sessionCancellationTokenSource = new CancellationTokenSource();
            workerCancellationToken.Register(() => sessionCancellationTokenSource.Cancel());

            return sessionCancellationTokenSource.Token;
        }

        private DateTime GetSimTime()
        {
            return DateTime.Now.Add(timeshift);
        }

        private void InitializeChecker(CancellationToken sessionCancellationToken)
        {
            checkerSettings = config
                .GetSection(nameof(IVUCrewChecker))
                .Get<IVUCrewChecker>();

            crewChecker.Initialize(
                host: checkerSettings.Host,
                port: checkerSettings.Port,
                path: checkerSettings.Path,
                username: checkerSettings.Username,
                password: checkerSettings.Password,
                isHttps: checkerSettings.IsHttps,
                division: checkerSettings.Division,
                planningLevel: checkerSettings.PlanningLevel,
                retryTime: checkerSettings.RetryTime);
        }

        private void InitializeConnector(CancellationToken sessionCancellationToken)
        {
            var connectorSettings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            databaseConnector.Initialize(
                connectionString: connectorSettings.ConnectionString,
                retryTime: connectorSettings.RetryTime,
                cancellationToken: sessionCancellationToken);
        }

        private void InitializeService()
        {
            var serviceSettings = config
                .GetSection(nameof(ServiceSettings))
                .Get<ServiceSettings>();

            queryDurationPast = new TimeSpan(
                hours: 0,
                minutes: serviceSettings.AbfrageInVergangenheitMin * -1,
                seconds: 0);

            queryDurationFuture = new TimeSpan(
                hours: 0,
                minutes: serviceSettings.AbfrageInZukunftMin,
                seconds: 0);

            serviceInterval = new TimeSpan(
                hours: 0,
                minutes: 0,
                seconds: serviceSettings.AbfrageIntervalSek);
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

            ivuSessionDate = currentSession.IVUDatum;
            timeshift = currentSession.Verschiebung;

            logger.LogDebug($"Die IVU-Sitzung läuft am {ivuSessionDate:yyyy-MM-dd}.");
        }

        #endregion Private Methods
    }
}