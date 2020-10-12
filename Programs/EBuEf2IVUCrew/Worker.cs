#pragma warning disable CA1031 // Do not catch general exception types

using Common.Enums;
using Common.Interfaces;
using EBuEf2IVUBase;
using EnumerableExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EBuEf2IVUCrew
{
    internal class Worker
        : WorkerBase
    {
        #region Private Fields

        private const string CrewingSeparator = ", ";

        private readonly ICrewChecker crewChecker;

        private TimeSpan queryDurationFuture;
        private TimeSpan queryDurationPast;
        private TimeSpan serviceInterval;

        #endregion Private Fields

        #region Public Constructors

        public Worker(IConfiguration config, IStateHandler sessionStateHandler, IDatabaseConnector databaseConnector,
            ICrewChecker crewChecker, ILogger<Worker> logger)
            : base(config, sessionStateHandler, databaseConnector, logger)
        {
            this.crewChecker = crewChecker;
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
                    "Die Nachrichtenempfänger, Datenbank-Verbindungen und IVU-Sender von EBuEf2IVUCrew werden zurückgesetzt.");

                var sessionCancellationToken = GetSessionCancellationToken(workerCancellationToken);

                InitializeService();
                InitializeConnector(sessionCancellationToken);

                await StartIVUSessionAsync();

                while (!sessionCancellationToken.IsCancellationRequested)
                {
                    if (currentState == SessionStates.IsRunning)
                    {
                        await CheckCrewsAsync(sessionCancellationToken);

                        try
                        {
                            await Task.Delay(
                                delay: serviceInterval,
                                cancellationToken: sessionCancellationToken);
                        }
                        catch (TaskCanceledException)
                        {
                            logger.LogInformation(
                                "EBuEf2IVUCrew wird beendet.");
                        }
                    }
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

            logger.LogDebug(
                "In der EBuEf-DB wurden {trainsCount} Züge für den Zeitraum zwischen {minTime} und {maxTime} gefunden.",
                trainRuns.Count(),
                minTime.ToString(@"hh\:mm"),
                maxTime.ToString(@"hh\:mm"));

            if (trainRuns.Any()
                && !sessionCancellationToken.IsCancellationRequested)
            {
                var tripNumbers = trainRuns
                    .Select(t => t.Zugnummer).ToArray();
                var crewingElements = await crewChecker.GetCrewingElementsAsync(
                    tripNumbers: tripNumbers,
                    date: ivuSessionDate,
                    cancellationToken: sessionCancellationToken);

                logger.LogDebug(
                    "In der IVU.rail wurden {crewingCount} Besatzungseinträge zu den Zügen gefunden: {crewingElements}",
                    crewingElements.Count(),
                    crewingElements.Merge(CrewingSeparator));

                if (crewingElements.Any()
                    && !sessionCancellationToken.IsCancellationRequested)
                {
                    await databaseConnector.SetCrewingsAsync(crewingElements);
                }
            }
        }

        private void InitializeService()
        {
            var serviceSettings = config
                .GetSection(nameof(CrewChecker))
                .Get<Settings.CrewChecker>();

            queryDurationPast = new TimeSpan(
                hours: 0,
                minutes: serviceSettings.AbfrageVergangenheitMin * -1,
                seconds: 0);

            queryDurationFuture = new TimeSpan(
                hours: 0,
                minutes: serviceSettings.AbfrageZukunftMin,
                seconds: 0);

            serviceInterval = new TimeSpan(
                hours: 0,
                minutes: 0,
                seconds: serviceSettings.AbfrageIntervalSek);

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

        #endregion Private Methods
    }
}

#pragma warning restore CA1031 // Do not catch general exception types