using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class BahnuebergaengeEinstellung
    {
        public int Id { get; set; }
        public int BahnuebergangId { get; set; }
        public int AufloesungId { get; set; }
        public int FahrstrasseId { get; set; }
        public int GleisId { get; set; }
        public string Art { get; set; }
        public int Status { get; set; }
        public int EinstellungTimestamp { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
