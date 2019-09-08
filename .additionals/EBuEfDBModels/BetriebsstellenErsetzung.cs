using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class BetriebsstellenErsetzung
    {
        public int Id { get; set; }
        public string Ursprung { get; set; }
        public string Ersatz { get; set; }
        public string Bezug { get; set; }
        public string Art { get; set; }
        public string Ursprung2 { get; set; }
        public string Ursprung3 { get; set; }

        public virtual BetriebsstellenListe BezugNavigation { get; set; }
        public virtual BetriebsstellenListe ErsatzNavigation { get; set; }
        public virtual BetriebsstellenListe Ursprung2Navigation { get; set; }
        public virtual BetriebsstellenListe Ursprung3Navigation { get; set; }
        public virtual BetriebsstellenListe UrsprungNavigation { get; set; }
    }
}
