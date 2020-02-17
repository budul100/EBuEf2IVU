using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface ICrewChecker
    {
        #region Public Methods

        Task<IEnumerable<CrewingElement>> GetCrewingElementsAsync(IEnumerable<string> tripNumbers,
            DateTime date, CancellationToken cancellationToken);

        void Initialize(string host, int port, string path, string username, string password, bool isHttps,
            string division, string planningLevel, int retryTime);

        #endregion Public Methods
    }
}