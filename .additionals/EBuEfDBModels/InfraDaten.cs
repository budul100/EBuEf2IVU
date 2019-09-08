using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class InfraDaten
    {
        public int Id { get; set; }
        public int InfraId { get; set; }
        public int? StoerungId { get; set; }
        public int? LinkedId { get; set; }
        public string LinkedArt { get; set; }
        public string WertArt { get; set; }
        public string Wert { get; set; }

        public virtual InfraZustand Infra { get; set; }
        public virtual InfraZustand Linked { get; set; }
        public virtual InfraZustand Stoerung { get; set; }
    }
}
