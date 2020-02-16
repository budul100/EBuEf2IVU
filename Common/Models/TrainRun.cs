using System;

namespace Common.Models
{
    public class TrainRun
    {
        #region Public Properties

        public TimeSpan? AbfahrtIst { get; set; }

        public TimeSpan? AbfahrtSollPlan { get; set; }

        public string Zugnummer { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return @$"Zug: {Zugnummer} | Abfahrt: {AbfahrtSollPlan:hh:mm:ss} | Ist-Abfahrt: {AbfahrtIst:hh:mm:ss}";
        }

        #endregion Public Methods
    }
}