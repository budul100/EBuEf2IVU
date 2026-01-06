using System;

namespace EBuEf2IVU.Shareds.Commons.EventsArgs
{
    public class MessageReceivedArgs(string content)
        : EventArgs
    {
        #region Public Properties

        public string Content { get; } = content;

        #endregion Public Properties
    }
}