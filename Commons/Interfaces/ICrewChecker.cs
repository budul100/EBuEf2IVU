using Commons.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Commons.Interfaces
{
    public interface ICrewChecker
    {
        #region Public Methods

        Task<IEnumerable<CrewingElement>> GetCrewingElementsAsync(IEnumerable<string> tripNumbers,
            DateTime date, CancellationToken cancellationToken);

        void Initialize(string host, int port, bool isHttps, string username, string password,
            string path, string division, string planningLevel, int retryTime);

        #endregion Public Methods
    }
}