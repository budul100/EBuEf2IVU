using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class StreckenAbschnitte
    {
        public int Id { get; set; }
        public int StreckenId { get; set; }
        public string Richtung { get; set; }
        public int Reihenfolge { get; set; }
        public string Betriebsstelle0 { get; set; }
        public string Betriebsstelle1 { get; set; }
        public int? Laenge { get; set; }

        public virtual BetriebsstellenListe Betriebsstelle0Navigation { get; set; }
        public virtual BetriebsstellenListe Betriebsstelle1Navigation { get; set; }
        public virtual Strecken Strecken { get; set; }
    }
}
