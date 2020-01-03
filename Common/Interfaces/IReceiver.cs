using Common.EventsArgs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IReceiver
    {
        #region Public Events

        event EventHandler<MessageReceivedArgs> MessageReceivedEvent;

        #endregion Public Events

        #region Public Methods

        Task RunAsync(CancellationToken cancellationToken);

        #endregion Public Methods
    }
}