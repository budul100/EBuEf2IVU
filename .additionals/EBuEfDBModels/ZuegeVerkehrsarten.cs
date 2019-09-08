using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class ZuegeVerkehrsarten
    {
        public ZuegeVerkehrsarten()
        {
            ZuegeZuggattungen = new HashSet<ZuegeZuggattungen>();
        }

        public int Id { get; set; }
        public string VerkehrsartKuerzel { get; set; }
        public string Beschreibung { get; set; }

        public virtual ICollection<ZuegeZuggattungen> ZuegeZuggattungen { get; set; }
    }
}
