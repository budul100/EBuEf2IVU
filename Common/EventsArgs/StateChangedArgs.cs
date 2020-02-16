using System;

namespace Common.EventsArgs
{
    public class StateChangedArgs
        : EventArgs
    {
        #region Public Constructors

        public StateChangedArgs(string status)
        {
            Status = status;
        }

        #endregion Public Constructors

        #region Public Properties

        public string Status { get; private set; }

        #endregion Public Properties
    }
}