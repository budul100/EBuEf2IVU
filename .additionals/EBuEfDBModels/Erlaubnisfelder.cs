using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class Erlaubnisfelder
    {
        public int Id { get; set; }
        public int ErlaubnisfeldId { get; set; }
        public string Betriebsstelle0 { get; set; }
        public string Betriebsstelle1 { get; set; }

        public virtual BetriebsstellenListe Betriebsstelle0Navigation { get; set; }
        public virtual BetriebsstellenListe Betriebsstelle1Navigation { get; set; }
        public virtual InfraZustand Erlaubnisfeld { get; set; }
    }
}
