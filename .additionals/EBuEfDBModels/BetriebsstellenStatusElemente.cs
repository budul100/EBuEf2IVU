using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class BetriebsstellenStatusElemente
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
        public int? InfraId { get; set; }
        public sbyte? InfraDir { get; set; }
        public int? SignalId { get; set; }
        public int? FahrstrasseId { get; set; }
        public int? ZielfeldId { get; set; }

        public virtual Fahrstrassen Fahrstrasse { get; set; }
        public virtual InfraZustand Infra { get; set; }
        public virtual Signale Signal { get; set; }
        public virtual BetriebsstellenStatus Status { get; set; }
        public virtual Gbt Zielfeld { get; set; }
    }
}
