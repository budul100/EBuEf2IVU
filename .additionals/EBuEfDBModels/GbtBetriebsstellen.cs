using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class GbtBetriebsstellen
    {
        public int Id { get; set; }
        public string StartBetriebsstelle { get; set; }
        public int StartGbtId { get; set; }
        public string ZielBetriebsstelle { get; set; }
        public string ZielBetriebsstelle2 { get; set; }
        public int ZielGbtId { get; set; }
        public int IstKurzeinfahrt { get; set; }
        public int IstVomGegengleis { get; set; }
        public int IstInsGegengleis { get; set; }

        public virtual BetriebsstellenListe StartBetriebsstelleNavigation { get; set; }
        public virtual Gbt StartGbt { get; set; }
        public virtual BetriebsstellenListe ZielBetriebsstelle2Navigation { get; set; }
        public virtual BetriebsstellenListe ZielBetriebsstelleNavigation { get; set; }
        public virtual Gbt ZielGbt { get; set; }
    }
}
