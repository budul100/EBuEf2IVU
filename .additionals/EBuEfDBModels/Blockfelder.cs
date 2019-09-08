using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class Blockfelder
    {
        public Blockfelder()
        {
            BlockfelderElemente = new HashSet<BlockfelderElemente>();
        }

        public int Id { get; set; }
        public int BlockfeldId { get; set; }
        public string Bezeichnung { get; set; }
        public int? ErlaubnisId { get; set; }
        public int ErlaubnisDir { get; set; }

        public virtual InfraZustand Blockfeld { get; set; }
        public virtual InfraZustand Erlaubnis { get; set; }
        public virtual ICollection<BlockfelderElemente> BlockfelderElemente { get; set; }
    }
}
