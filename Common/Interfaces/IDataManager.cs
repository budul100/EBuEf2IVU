using Common.BusinessObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IDataManager
    {
        #region Public Methods

        Task<IEnumerable<VehicleAllocation>> GetVehicleAllocationsAsync();

        void Run(string connectionString, int retryTime);

        void SetTrainPosition(TrainPosition position);

        #endregion Public Methods
    }
}