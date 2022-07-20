using Common.Enums;
using System;

namespace Common.EventsArgs
{
    public class StateChangedArgs
        : EventArgs
    {
        #region Public Constructors

        public StateChangedArgs(StateType stateType)
        {
            StateType = stateType;
        }

        #endregion Public Constructors

        #region Public Properties

        public StateType StateType { get; }

        #endregion Public Properties
    }
}