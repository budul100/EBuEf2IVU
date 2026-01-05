using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Commons.Enums;
using Commons.Extensions;
using Commons.Interfaces;
using EnumerableExtensions;

namespace EBuEf2IVUCrew
{
    public class Worker(IConfiguration config, IStateHandler sessionStateHandler,
        IDatabaseConnector databaseConnector, ICrewChecker crewChecker, ILogger<Worker> logger)
        : EBuEf2IVUBase.Worker(config: config, sessionStateHandler: sessionStateHandler,
            databaseConnector: databaseConnector, logger: logger, assembly: Assembly.GetExecutingAssembly())
    {
        #region Private Fields

        private TimeSpan queryDurationFuture;
        private TimeSpan queryDurationPast;
        private TimeSpan serviceInterval;

        #endregion Private Fields

        #region Protected Methods

        protected override async Task ExecuteAsync(CancellationToken workerCancellationToken)
        {
            _ = InitializeConnectionAsync(workerCancellationToken);

            InitializeCrewChecker();

            while (!workerCancellationToken.IsCancellationRequested)
            {
                _ = HandleSessionStateAsync(
                    stateType: sessionStateHandler.StateType);

                var sessionCancellationToken = GetSessionCancellationToken(
                    workerCancellationToken: workerCancellationToken);

                while (!sessionCancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (sessionStateHandler?.StateType == StateType.IsRunning)
                        {
                            await CheckCrewsAsync(
                                sessionCancellationToken: sessionCancellationToken);

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

                base.logger.LogInformation(
                    "EBuEf2IVUCrew wird gestoppt.");
            }
        }

        protected override async Task HandleSessionStateAsync(StateType stateType)
        {
            if (stateType == StateType.IsRunning)
            {
                await InitializeSessionAsync();

                base.logger.LogDebug(
                    "Die EBuEf-DB wird für den Crew-Check alle {interval} Sekunden " +
                    "nach Zügen im Zeitraum von -{minTime} und +{maxTime} abgefragt.",
                    serviceInterval.TotalSeconds,
                    queryDurationPast.ToString(@"hh\:mm"),
                    queryDurationFuture.ToString(@"hh\:mm"));
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private async Task CheckCrewsAsync(CancellationToken sessionCancellationToken)
        {
            var simTime = ebuefSession.GetSimTime();

            var minTime = simTime
                .Add(queryDurationPast).TimeOfDay;
            var maxTime = simTime
                .Add(queryDurationFuture).TimeOfDay;

            var trainRuns = await databaseConnector.GetTrainRunsDispoAsync(
                minTime: minTime,
                maxTime: maxTime);

            base.logger.LogDebug(
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
                    date: ivuDatum,
                    cancellationToken: sessionCancellationToken);

                base.logger.LogDebug(
                    "In der IVU.rail wurden {crewingCount} Besatzungseinträge zu den Zügen gefunden: {crewingElements}",
                    crewingElements?.Count() ?? 0,
                    crewingElements?.Merge());

                if (crewingElements.AnyItem()
                    && !sessionCancellationToken.IsCancellationRequested)
                {
                    await databaseConnector.SetCrewingsAsync(
                        crewingElements: crewingElements);
                }
            }
        }

        private void InitializeCrewChecker()
        {
            base.logger.LogInformation(
                "IVU-Connector für EBuEf2IVUCrew wird gestartet.");

            var settings = config
                .GetSection(nameof(Commons.Settings.CrewChecker))
                .Get<Commons.Settings.CrewChecker>();

            serviceInterval = new TimeSpan(
                hours: 0,
                minutes: 0,
                seconds: settings.AbfrageIntervalSek);

            queryDurationPast = new TimeSpan(
                hours: 0,
                minutes: settings.AbfrageVergangenheitMin * -1,
                seconds: 0);

            queryDurationFuture = new TimeSpan(
                hours: 0,
                minutes: settings.AbfrageZukunftMin,
                seconds: 0);

            var host = settings.GetIVUAppServerHost();
            var port = settings.GetIVUAppServerPort();
            var isHttps = settings.IsIVUAppServerHttps();

            crewChecker.Initialize(
                host: host,
                port: port,
                isHttps: isHttps,
                username: settings.Username,
                password: settings.Password,
                path: settings.Path,
                division: settings.Division,
                planningLevel: settings.PlanningLevel,
                retryTime: settings.RetryTime);
        }

        #endregion Private Methods
    }
}