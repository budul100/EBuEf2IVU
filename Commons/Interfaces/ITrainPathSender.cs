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

        void Add(IEnumerable<TrainRun> trainRuns);

        Task ExecuteAsync(DateTime ivuDatum, string sessionKey, CancellationToken cancellationToken);

        void Initialize(string host, int port, string path, string username, string password, bool isHttps,
            int retryTime, string infrastructureManager, string orderingTransportationCompany, string stoppingReasonStop,
            string stoppingReasonPass, string trainPathStateRun, string trainPathStateCancelled, string importProfile,
            IEnumerable<string> ignoreTrainTypes, IEnumerable<string> locationShortnames, bool logRequests);

        #endregion Public Methods
    }
}