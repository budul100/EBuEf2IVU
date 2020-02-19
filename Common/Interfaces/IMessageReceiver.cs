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

        void Initialize(string ipAdress, int port, int retryTime, string messageType = default);

        Task RunAsync(CancellationToken cancellationToken);

        #endregion Public Methods
    }
}