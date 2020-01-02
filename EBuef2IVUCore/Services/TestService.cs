using Common.BusinessObjects;
using Common.Interfaces;
using Common.Settings;
using Serilog;
using System;
using System.Threading;

namespace EBuEf2IVUCore.Services
{
    internal static class TestService
    {
        #region Public Methods

        public static void RunPerformanceTest(ILogger logger, IDataManager dataManager, EBuEf2IVUSettings settings, int rounds)
        {
            dataManager.Run(
                connectionString: settings.DatabaseConnectionString,
                retryTime: settings.DatabaseRetryTime);

            for (var index = 0; index < rounds; index++)
            {
                var vonPosition = new TrainPosition
                {
                    Zugnummer = settings.PerformanceTestZugnummer,
                    EBuEfBetriebsstelleVon = settings.PerformanceTestBetriebsstelle,
                    EBuEfZeitpunktVon = DateTime.Now.TimeOfDay,
                };

                dataManager.AddRealtime(vonPosition);

                Thread.Sleep(settings.PerformanceTestSleep);

                var nachPosition = new TrainPosition
                {
                    Zugnummer = settings.PerformanceTestZugnummer,
                    EBuEfBetriebsstelleNach = settings.PerformanceTestBetriebsstelle,
                    EBuEfZeitpunktVon = DateTime.Now.TimeOfDay,
                };

                dataManager.AddRealtime(nachPosition);

                Thread.Sleep(settings.PerformanceTestSleep);

                logger.Debug($"{index}. Runde durchgeführt.");
            }
        }

        #endregion Public Methods
    }
}