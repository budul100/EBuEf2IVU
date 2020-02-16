using Common.Enums;
using System;

namespace Common.EventsArgs
{
    public class StateChangedArgs
        : EventArgs
    {
        #region Public Constructors

        public StateChangedArgs(SessionStates state)
        {
            State = state;
        }

        #endregion Public Constructors

        #region Public Properties

        public SessionStates State { get; private set; }

        #endregion Public Properties
    }
}