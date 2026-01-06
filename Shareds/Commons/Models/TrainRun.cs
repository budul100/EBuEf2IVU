using System;
using System.Collections.Generic;

namespace EBuEf2IVU.Shareds.Commons.Models
{
    public class TrainRun
    {
        #region Public Properties

        public TimeSpan? Abfahrt { get; set; }

        public string Bemerkungen { get; set; }

        public bool IstGeaendert { get; set; }

        public IEnumerable<TrainPosition> Positions { get; set; }

        public string Zuggattung { get; set; }

        public int ZugId { get; set; }

        public int Zugnummer { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return @$"Zug: {Zugnummer} ({ZugId}) | Abfahrt: {Abfahrt:hh\:mm\:ss}";
        }

        #endregion Public Methods
    }
}