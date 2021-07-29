using Common.EventsArgs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IStateHandler
    {
        #region Public Events

        event EventHandler<StateChangedArgs> SessionChangedEvent;

        event EventHandler SessionStartedEvent;

        #endregion Public Events

        #region Public Methods

        void Initialize(string host, int port, int retryTime, string startPattern, string statusPattern);

        Task RunAsync(CancellationToken cancellationToken);

        #endregion Public Methods
    }
}