using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface ISender
    {
        #region Public Methods

        void AddAllocations(IEnumerable<VehicleAllocation> allocations, DateTime startTime);

        void AddRealtime(TrainPosition position);

        Task RunAsnc(CancellationToken cancellationToken);

        #endregion Public Methods
    }
}