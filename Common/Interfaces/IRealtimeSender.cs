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

        void AddAllocations(IEnumerable<VehicleAllocation> allocations, DateTime startTime);

        void AddRealtime(TrainLeg leg);

        void Initialize(string division, string endpoint, int retryTime);

        Task RunAsnc(CancellationToken cancellationToken);

        #endregion Public Methods
    }
}