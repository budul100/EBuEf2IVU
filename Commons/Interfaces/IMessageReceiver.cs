using System;
using System.Threading;
using System.Threading.Tasks;
using Commons.EventsArgs;

namespace Commons.Interfaces
{
    public interface IMessageReceiver
    {
        #region Public Events

        event EventHandler<MessageReceivedArgs> MessageReceivedEvent;

        #endregion Public Events

        #region Public Methods

        Task ExecuteAsync(CancellationToken cancellationToken);

        #endregion Public Methods
    }
}