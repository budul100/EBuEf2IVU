using System;
using System.Collections.Generic;

namespace Common.Models
{
    public class TrainRun
    {
        #region Public Properties

        public TimeSpan? Abfahrt { get; set; }

        public string Bemerkungen { get; set; }

        public DateTime IVUDatum { get; set; }

        public IEnumerable<TrainPosition> Positions { get; set; }

        public string SessionKey { get; set; }

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