using Commons.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Commons.Interfaces
{
    public interface IRealtimeSender
    {
        #region Public Methods

        void Add(IEnumerable<VehicleAllocation> trainAllocations);

        void Add(TrainLeg trainLeg);

        Task ExecuteAsync(DateTime ivuDatum, TimeSpan sessionStart, CancellationToken cancellationToken);

        void Initialize(string host, int port, bool isHttps, string username, string password, string path,
            string division, int retryTime);

        #endregion Public Methods
    }
}