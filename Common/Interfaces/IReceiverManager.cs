using Common.EventsArgs;
using System;

namespace Common.Interfaces
{
    public interface IReceiverManager
    {
        #region Public Events

        event EventHandler<RealTimeReceivedArgs> RealTimeReceivedEvent;

        event EventHandler SessionStartedEvent;

        #endregion Public Events

        #region Public Methods

        void Run(string allocationsIpAdress, int allocationsListenerPort, int allocationsRetryTime, string allocationsPattern,
            string messagesIpAdress, int messagesListenerPort, int messagesRetryTime);

        #endregion Public Methods
    }
}