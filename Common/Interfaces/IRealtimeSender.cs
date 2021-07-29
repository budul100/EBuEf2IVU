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

        void Add(IEnumerable<VehicleAllocation> trainAllocations);

        void Add(TrainLeg trainLeg);

        Task ExecuteAsync(CancellationToken cancellationToken);

        void Initialize(string division, string endpoint, int retryTime, DateTime sessionStart);

        #endregion Public Methods
    }
}