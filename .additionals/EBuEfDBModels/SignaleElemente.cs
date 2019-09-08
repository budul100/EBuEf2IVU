using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class SignaleElemente
    {
        public int Id { get; set; }
        public int SignalId { get; set; }
        public int InfraId { get; set; }
        public int Dir { get; set; }

        public virtual InfraZustand Infra { get; set; }
        public virtual SignaleBegriffe Signal { get; set; }
    }
}
