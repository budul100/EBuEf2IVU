using Common.Enums;
using System;

namespace Common.EventsArgs
{
    public class StateChangedArgs
        : EventArgs
    {
        #region Public Constructors

        public StateChangedArgs(SessionStatusType state)
        {
            State = state;
        }

        #endregion Public Constructors

        #region Public Properties

        public SessionStatusType State { get; private set; }

        #endregion Public Properties
    }
}