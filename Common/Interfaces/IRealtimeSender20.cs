using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IRealtimeSender20
    {
        #region Public Methods

        void Add(IEnumerable<VehicleAllocation> trainAllocations);

        void Add(TrainLeg trainLeg);

        Task ExecuteAsync(CancellationToken cancellationToken);

        void Initialize(string endpoint, string division, DateTime sessionStart, int retryTime);

        #endregion Public Methods
    }
}