using System;

namespace EBuEf2IVU.Shareds.Commons.Models
{
    public class TrainPosition
    {
        #region Public Properties

        public DateTime? Abfahrt { get; set; }

        public DateTime? Ankunft { get; set; }

        public string Bemerkungen { get; set; }

        public string Betriebsstelle { get; set; }

        public string Gleis { get; set; }

        public bool IstDurchfahrt { get; set; }

        public bool VerkehrNicht { get; set; }

        #endregion Public Properties
    }
}