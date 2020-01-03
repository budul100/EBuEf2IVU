using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IConnector
    {
        #region Public Methods

        Task AddRealtimeAsync(TrainPosition position);

        Task<DateTime> GetSessionDateAsync();

        Task<IEnumerable<VehicleAllocation>> GetVehicleAllocationsAsync();

        #endregion Public Methods
    }
}