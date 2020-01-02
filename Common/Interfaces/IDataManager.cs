using Common.BusinessObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IDataManager
    {
        #region Public Methods

        void AddRealtime(TrainPosition position);

        Task<IEnumerable<VehicleAllocation>> GetVehicleAllocationsAsync();

        void Run(string connectionString, int retryTime);

        #endregion Public Methods
    }
}