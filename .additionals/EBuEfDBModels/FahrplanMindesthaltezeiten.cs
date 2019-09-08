using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrplanMindesthaltezeiten
    {
        public int Id { get; set; }
        public string Betriebsstelle { get; set; }
        public int? Gleis { get; set; }
        public int? ZuggattungId { get; set; }
        public int Mindesthaltezeit { get; set; }

        public virtual BetriebsstellenListe BetriebsstelleNavigation { get; set; }
        public virtual ZuegeZuggattungen Zuggattung { get; set; }
    }
}
