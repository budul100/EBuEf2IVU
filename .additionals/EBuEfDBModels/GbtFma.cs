using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class GbtFma
    {
        public int Id { get; set; }
        public int? GbtId { get; set; }
        public int? FmaId { get; set; }
        public int InfraId { get; set; }

        public virtual Fma Fma { get; set; }
        public virtual Gbt Gbt { get; set; }
        public virtual InfraZustand Infra { get; set; }
    }
}
