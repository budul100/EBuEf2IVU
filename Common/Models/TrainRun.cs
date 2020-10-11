using System;
using System.Collections.Generic;

namespace Common.Models
{
    public class TrainRun
    {
        #region Public Properties

        public TimeSpan? Abfahrt { get; set; }

        public IEnumerable<TrainPosition> Positions { get; set; }

        public string Zugnummer { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return @$"Zug: {Zugnummer} | Abfahrt: {Abfahrt:hh\:mm\:ss}";
        }

        #endregion Public Methods
    }
}