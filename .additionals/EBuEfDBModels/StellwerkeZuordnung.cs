using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class StellwerkeZuordnung
    {
        public int Id { get; set; }
        public int StellwerkUzId { get; set; }
        public int? InfraId { get; set; }
        public int? SignalId { get; set; }
        public string Art { get; set; }

        public virtual InfraZustand Infra { get; set; }
        public virtual Signale Signal { get; set; }
        public virtual StellwerkeUz StellwerkUz { get; set; }
    }
}
