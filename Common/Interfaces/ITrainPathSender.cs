using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface ITrainPathSender
    {
        #region Public Methods

        void AddTrain(TrainRun trainRun);

        void AddTrains(IEnumerable<TrainRun> trainRuns);

        void Initialize(string host, int port, string path, string username, string password, bool isHttps,
            int retryTime, DateTime sessionDate, string infrastructureManager, string orderingTransportationCompany,
            string trainPathState, string stoppingReasonStop, string stoppingReasonPass, string importProfile);

        Task RunAsnc(CancellationToken cancellationToken);

        #endregion Public Methods
    }
}