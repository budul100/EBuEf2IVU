using Common.Enums;
using Common.EventsArgs;
using Common.Extensions;
using Common.Interfaces;
using EBuEf2IVUBase;
using EnumerableExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EBuEf2IVUCrew
{
    public class Worker
        : WorkerBase
    {
        #region Private Fields

        private readonly ICrewChecker crewChecker;

        private bool isSessionInitialized;
        private TimeSpan queryDurationFuture;
        private TimeSpan queryDurationPast;
        private TimeSpan serviceInterval;

        #endregion Private Fields

        #region Public Constructors

        public Worker(IConfiguration config, IStateHandler sessionStateHandler, IDatabaseConnector databaseConnector,
            ICrewChecker crewChecker, ILogger<Worker> logger)
            : base(config: config, sessionStateHandler: sessionStateHandler,
                  databaseConnector: databaseConnector, logger: logger,
                  assembly: Assembly.GetExecutingAssembly())
        {
            this.sessionStateHandler.SessionChangedEvent += OnSessionChangedAsync;

            this.crewChecker = crewChecker;
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override async Task ExecuteAsync(CancellationToken workerCancellationToken)
        {
            _ = InitializeConnectionAsync(workerCancellationToken);

            InitializeCrewChecker();

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
                            await CheckCrewsAsync(sessionCancellationToken);

                            await Task.Delay(
                                delay: serviceInterval,
                                cancellationToken: sessionCancellationToken);
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

                isSessionInitialized = false;

                logger.LogInformation(
                    "EBuEf2IVUCrew wird gestoppt.");
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private async Task CheckCrewsAsync(CancellationToken sessionCancellationToken)
        {
            var minTime = ebuefSession.GetSimTime()
                .Add(queryDurationPast).TimeOfDay;
            var maxTime = ebuefSession.GetSimTime()
                .Add(queryDurationFuture).TimeOfDay;

            var trainRuns = await databaseConnector.GetTrainRunsDispoAsync(
                minTime: minTime,
                maxTime: maxTime);

            logger.LogDebug(
                "In der EBuEf-DB wurden {trainsCount} Züge für den Zeitraum zwischen {minTime} und {maxTime} gefunden.",
                trainRuns.Count(),
                minTime.ToString(@"hh\:mm"),
                maxTime.ToString(@"hh\:mm"));

            if (trainRuns.Any()
                && !sessionCancellationToken.IsCancellationRequested)
            {
                var tripNumbers = trainRuns
                    .Select(t => t.Zugnummer.ToString()).ToArray();
                var crewingElements = await crewChecker.GetCrewingElementsAsync(
                    tripNumbers: tripNumbers,
                    date: ivuSessionDate,
                    cancellationToken: sessionCancellationToken);

                logger.LogDebug(
                    "In der IVU.rail wurden {crewingCount} Besatzungseinträge zu den Zügen gefunden: {crewingElements}",
                    crewingElements.Count(),
                    crewingElements.Merge());

                if (crewingElements.Any()
                    && !sessionCancellationToken.IsCancellationRequested)
                {
                    await databaseConnector.SetCrewingsAsync(crewingElements);
                }
            }
        }

        private async Task HandleSessionStateAsync(StateType stateType)
        {
            if (stateType == StateType.IsEnded
                || stateType == StateType.IsPaused)
            {
                isSessionInitialized = false;
            }
            else if (stateType == StateType.IsRunning
                && !isSessionInitialized)
            {
                await InitializeSessionAsync();

                logger.LogDebug(
                    "Die EBuEf-DB wird für den Crew-Check alle {interval} Sekunden " +
                    "nach Zügen im Zeitraum von -{minTime} und +{maxTime} abgefragt.",
                    serviceInterval.TotalSeconds,
                    queryDurationPast.ToString(@"hh\:mm"),
                    queryDurationFuture.ToString(@"hh\:mm"));

                isSessionInitialized = true;
            }
        }

        private void InitializeCrewChecker()
        {
            logger.LogInformation(
                "IVU-Connector für EBuEf2IVUCrew wird gestartet.");

            var serviceSettings = config
                .GetSection(nameof(Settings.CrewChecker))
                .Get<Settings.CrewChecker>();

            serviceInterval = new TimeSpan(
                hours: 0,
                minutes: 0,
                seconds: serviceSettings.AbfrageIntervalSek);

            queryDurationPast = new TimeSpan(
                hours: 0,
                minutes: serviceSettings.AbfrageVergangenheitMin * -1,
                seconds: 0);

            queryDurationFuture = new TimeSpan(
                hours: 0,
                minutes: serviceSettings.AbfrageZukunftMin,
                seconds: 0);

            crewChecker.Initialize(
                host: serviceSettings.Host,
                port: serviceSettings.Port,
                path: serviceSettings.Path,
                username: serviceSettings.Username,
                password: serviceSettings.Password,
                isHttps: serviceSettings.IsHttps,
                division: serviceSettings.Division,
                planningLevel: serviceSettings.PlanningLevel,
                retryTime: serviceSettings.RetryTime);
        }

        private async void OnSessionChangedAsync(object sender, StateChangedArgs e)
        {
            await HandleSessionStateAsync(e.StateType);
        }

        #endregion Private Methods
    }
}