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

        void Add(IEnumerable<VehicleAllocation> trainAllocations);

        void Add(TrainLeg trainLeg);

        Task ExecuteAsync(DateTime ivuDatum, TimeSpan sessionStart, CancellationToken cancellationToken);

        void Initialize(string endpoint, string division, int retryTime);

        #endregion Public Methods
    }
}