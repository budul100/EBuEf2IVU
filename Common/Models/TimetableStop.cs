using System;

namespace Common.Models
{
    public class TimetableStop
    {
        #region Public Properties

        public TimeSpan? Abfahrt { get; set; }

        public TimeSpan? Ankunft { get; set; }

        public string Betriebsstelle { get; set; }

        public string Zugnummer { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return @$"Betriebsstelle: {Betriebsstelle} | Zug: {Zugnummer} | Abfahrt: {Abfahrt:hh\:mm\:ss}";
        }

        #endregion Public Methods
    }
}