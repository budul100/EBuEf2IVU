using System;
using System.Collections.Generic;

namespace Common.Models
{
    public class TrainPosition
    {
        #region Public Properties

        public string EBuEfBetriebsstelleNach { get; set; }

        public string EBuEfBetriebsstelleVon { get; set; }

        public string EBuEfGleisNach { get; set; }

        public string EBuEfGleisVon { get; set; }

        public TimeSpan EBuEfZeitpunktNach { get; set; }

        public TimeSpan EBuEfZeitpunktVon { get; set; }

        public IEnumerable<string> Fahrzeuge { get; set; }

        public string IVUGleis { get; set; }

        public string IVUNetzpunkt { get; set; }

        public TrainPositionType IVUTrainPositionTyp { get; set; }

        public DateTime IVUZeitpunkt { get; set; }

        public string Zugnummer { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return $@"Zug: {Zugnummer.ToString()} | EBuEf von: {EBuEfBetriebsstelleVon}/{EBuEfGleisVon} um {EBuEfZeitpunktVon:hh\:mm} | " +
                $@"EBuEf nach: {EBuEfBetriebsstelleNach}/{EBuEfGleisNach} um {EBuEfZeitpunktNach:hh\:mm} | " +
                $@"IVU: {IVUNetzpunkt}/{IVUGleis} um {IVUZeitpunkt:HH\:mm} ({IVUTrainPositionTyp.ToString()})";
        }

        #endregion Public Methods
    }
}