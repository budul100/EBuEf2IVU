using Commons.EventsArgs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Commons.Interfaces
{
    public interface IMessageReceiver
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