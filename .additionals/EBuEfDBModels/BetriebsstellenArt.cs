using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class BetriebsstellenArt
    {
        public BetriebsstellenArt()
        {
            BetriebsstellenListe = new HashSet<BetriebsstellenListe>();
        }

        public int Id { get; set; }
        public string Art { get; set; }
        public string Artname { get; set; }
        public int Zuganfang { get; set; }

        public virtual ICollection<BetriebsstellenListe> BetriebsstellenListe { get; set; }
    }
}
