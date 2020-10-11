using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IDatabaseConnector
    {
        #region Public Methods

        Task AddRealtimeAsync(TrainLeg leg);

        Task<EBuEfSession> GetEBuEfSessionAsync();

        Task<TrainRun> GetTrainRunAsync(string trainNumber);

        Task<IEnumerable<TrainRun>> GetTrainRunsAsync(TimeSpan minTime, TimeSpan maxTime);

        Task<IEnumerable<VehicleAllocation>> GetVehicleAllocationsAsync();

        void Initialize(string connectionString, int retryTime, CancellationToken cancellationToken);

        Task SetCrewingsAsync(IEnumerable<CrewingElement> crewingElements);

        #endregion Public Methods
    }
}