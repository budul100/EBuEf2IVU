using System;
using EBuEf2IVU.Shareds.Commons.Enums;

namespace EBuEf2IVU.Shareds.Commons.EventsArgs
{
    public class StateChangedArgs(StateType stateType)
        : EventArgs
    {
        #region Public Properties

        public StateType StateType { get; } = stateType;

        #endregion Public Properties
    }
}