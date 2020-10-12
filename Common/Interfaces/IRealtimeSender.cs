using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IRealtimeSender
    {
        #region Public Methods

        void AddAllocations(IEnumerable<VehicleAllocation> trainAllocations);

        void AddRealtime(TrainLeg trainLeg);

        void Initialize(string division, string endpoint, int retryTime, DateTime sessionStart);

        Task RunAsnc(CancellationToken cancellationToken);

        #endregion Public Methods
    }
}