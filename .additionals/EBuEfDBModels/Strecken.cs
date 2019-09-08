using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class Strecken
    {
        public Strecken()
        {
            StreckenAbschnitte = new HashSet<StreckenAbschnitte>();
        }

        public int Id { get; set; }
        public string Nummer { get; set; }
        public string Beschreibung { get; set; }
        public int IstBahnhofsstrecke { get; set; }

        public virtual ICollection<StreckenAbschnitte> StreckenAbschnitte { get; set; }
    }
}
