using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class StellwerkeMerkschilder
    {
        public int Id { get; set; }
        public int InfraId { get; set; }
        public string Merktext { get; set; }
        public int? Befahrbarkeitssperre { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual InfraZustand Infra { get; set; }
    }
}
