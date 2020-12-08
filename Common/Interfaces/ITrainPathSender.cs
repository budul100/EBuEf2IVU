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

        void Add(IEnumerable<TrainPathMessage> messages);

        void Add(IEnumerable<TrainRun> trainRuns);

        void Initialize(string host, int port, string path, string username, string password, bool isHttps,
            int retryTime, string sessionKey, DateTime sessionDate, string infrastructureManager,
            string orderingTransportationCompany, string stoppingReasonStop, string stoppingReasonPass,
            string trainPathStateRun, string trainPathStateCancelled, string importProfile, bool preferPrognosis,
            IEnumerable<string> ignoreTrainTypes, IEnumerable<string> locationShortnames);

        Task RunAsnc(CancellationToken cancellationToken);

        #endregion Public Methods
    }
}