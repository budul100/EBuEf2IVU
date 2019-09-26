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
                var position = new TrainPosition
                {
                    Zugnummer = settings.PerformanceTestZugnummer,
                    EBuEfBetriebsstelleVon = settings.PerformanceTestBetriebsstelleVon,
                    EBuEfZeitpunktVon = DateTime.Now.TimeOfDay,
                };

                dataManager.SetTrainPosition(position);

                Thread.Sleep(100);

                logger.Debug($"{index}. Runde durchgeführt.");
            }
        }

        #endregion Public Methods
    }
}