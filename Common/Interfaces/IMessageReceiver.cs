using Common.EventsArgs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IMessageReceiver
    {
        #region Public Events

        event EventHandler<MessageReceivedArgs> MessageReceivedEvent;

        #endregion Public Events

        #region Public Methods

        void Initialize(string host, int port, int retryTime, string messageType);

        Task RunAsync(CancellationToken cancellationToken);

        #endregion Public Methods
    }
}