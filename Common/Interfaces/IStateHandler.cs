using Common.EventsArgs;
using System;
using System.Threading;

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

        void Run(CancellationToken cancellationToken);

        #endregion Public Methods
    }
}