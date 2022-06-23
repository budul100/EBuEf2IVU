using Common.Enums;
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

        #endregion Public Events

        #region Public Properties

        SessionStatusType SessionStatus { get; }

        #endregion Public Properties

        #region Public Methods

        Task ExecuteAsync(CancellationToken cancellationToken);

        void Initialize(string host, int port, int retryTime, string startPattern, string statusPattern);

        #endregion Public Methods
    }
}