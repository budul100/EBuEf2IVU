using System;
using System.Threading;
using System.Threading.Tasks;
using Commons.EventsArgs;

namespace Commons.Interfaces
{
    public interface IMulticastReceiver
    {
        #region Public Events

        event EventHandler<MessageReceivedArgs> MessageReceivedEvent;

        #endregion Public Events

        #region Public Methods

        Task ExecuteAsync(CancellationToken cancellationToken);

        void Initialize(string host, int port, int retryTime, string messageType);

        #endregion Public Methods
    }
}