using Common.Models;
using System;

namespace Common.EventsArgs
{
    public class RealTimeReceivedArgs : EventArgs
    {
        #region Public Properties

        public RealTimeMessage RealTimeMessage { get; set; }

        #endregion Public Properties
    }
}