using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class BetriebsstellenHinweise
    {
        public int Id { get; set; }
        public string Bezug { get; set; }
        public string Von { get; set; }
        public string Nach { get; set; }
        public string Gleis { get; set; }
        public int ZielStreckengleis { get; set; }
        public string Hinweistext { get; set; }

        public virtual BetriebsstellenListe BezugNavigation { get; set; }
        public virtual BetriebsstellenListe NachNavigation { get; set; }
        public virtual BetriebsstellenListe VonNavigation { get; set; }
    }
}
