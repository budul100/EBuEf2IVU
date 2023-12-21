using Commons.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Commons.Interfaces
{
    public interface ITrainPathSender
    {
        #region Public Methods

        void Add(IEnumerable<TrainRun> trainRuns);

        Task ExecuteAsync(DateTime ivuDatum, string sessionKey, CancellationToken cancellationToken);

        void Initialize(string host, int port, bool isHttps, string username, string password, string path,
            int retryTime, string infrastructureManager, string orderingTransportationCompany, string stoppingReasonStop,
            string stoppingReasonPass, string trainPathStateRun, string trainPathStateCancelled, string importProfile,
            IEnumerable<string> ignoreTrainTypes, IEnumerable<string> locationShortnames, bool logRequests);

        #endregion Public Methods
    }
}