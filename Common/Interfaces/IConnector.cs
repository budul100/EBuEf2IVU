using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IConnector
    {
        #region Public Methods

        Task AddRealtimeAsync(TrainPosition position);

        Task<EBuEfSession> GetEBuEfSessionAsync();

        Task<IEnumerable<TrainRun>> GetTrainRunsAsync(TimeSpan minTime, TimeSpan maxTime);

        Task<IEnumerable<VehicleAllocation>> GetVehicleAllocationsAsync();

        void Initialize(string connectionString, int retryTime, CancellationToken cancellationToken);

        #endregion Public Methods
    }
}