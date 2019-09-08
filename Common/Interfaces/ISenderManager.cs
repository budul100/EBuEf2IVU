using Common.BusinessObjects;
using System.Collections.Generic;

namespace Common.Interfaces
{
    public interface ISenderManager
    {
        #region Public Methods

        void AddAllocations(IEnumerable<VehicleAllocation> allocations);

        void AddRealtime(TrainPosition position);

        void Run(int retryTime, string ivuEndpoint, string ivuDivision);

        #endregion Public Methods
    }
}