using System;

namespace Common.Models
{
    public class TrainPosition
    {
        #region Public Properties

        public TimeSpan? Abfahrt { get; set; }

        public TimeSpan? Ankunft { get; set; }

        public string Bemerkungen { get; set; }

        public string Betriebsstelle { get; set; }

        public string Gleis { get; set; }

        public bool IstDurchfahrt { get; set; }

        #endregion Public Properties
    }
}