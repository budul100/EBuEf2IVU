using System;

namespace Common.EventsArgs
{
    public class MessageReceivedArgs
        : EventArgs
    {
        #region Public Properties

        public string Content { get; set; }

        #endregion Public Properties
    }
}