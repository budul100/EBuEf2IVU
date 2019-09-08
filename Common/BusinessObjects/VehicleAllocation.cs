using System.Collections.Generic;

namespace Common.BusinessObjects
{
    public class VehicleAllocation
    {
        #region Public Properties

        public string Betriebsstelle { get; set; }

        public IEnumerable<string> Fahrzeuge { get; set; }

        public string Gleis { get; set; }

        public string Zugnummer { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return $"Betriebsstelle: {Betriebsstelle} | Gleis: {Gleis} | Zugnummer: {Zugnummer}";
        }

        #endregion Public Methods
    }
}