using System;

namespace MessageReceiver.Models
{
    internal class MessageReceivedArgs : EventArgs
    {
        #region Public Properties

        public string Content { get; set; }

        #endregion Public Properties
    }
}