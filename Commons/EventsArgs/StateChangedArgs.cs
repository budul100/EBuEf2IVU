using System;
using Commons.Enums;

namespace Commons.EventsArgs
{
    public class StateChangedArgs(StateType stateType)
        : EventArgs
    {
        #region Public Properties

        public StateType StateType { get; } = stateType;

        #endregion Public Properties
    }
}