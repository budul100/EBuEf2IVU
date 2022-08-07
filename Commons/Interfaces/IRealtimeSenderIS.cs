using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IRealtimeSenderIS
    {
        #region Public Methods

        void Add(IEnumerable<VehicleAllocation> trainAllocations, DateTime sessionStart);

        void Add(TrainLeg trainLeg);

        Task ExecuteAsync(CancellationToken cancellationToken);

        void Initialize(string endpoint, string division, int retryTime);

        #endregion Public Methods
    }
}