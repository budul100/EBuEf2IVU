using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class BetriebsstellenStatus
    {
        public BetriebsstellenStatus()
        {
            BetriebsstellenStatusElemente = new HashSet<BetriebsstellenStatusElemente>();
        }

        public int Id { get; set; }
        public string Kuerzel { get; set; }
        public string Status { get; set; }

        public virtual BetriebsstellenListe KuerzelNavigation { get; set; }
        public virtual ICollection<BetriebsstellenStatusElemente> BetriebsstellenStatusElemente { get; set; }
    }
}
