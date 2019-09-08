using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class BlockfelderElemente
    {
        public int Id { get; set; }
        public int BlockfeldId { get; set; }
        public int InfraId { get; set; }

        public virtual Blockfelder Blockfeld { get; set; }
        public virtual InfraZustand Infra { get; set; }
    }
}
