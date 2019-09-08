using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class BetriebsstellenFahrtrichtung
    {
        public int Id { get; set; }
        public string Nachbar { get; set; }
        public string Bezug { get; set; }
        public int Fahrtrichtung { get; set; }

        public virtual BetriebsstellenListe BezugNavigation { get; set; }
        public virtual BetriebsstellenListe NachbarNavigation { get; set; }
    }
}
