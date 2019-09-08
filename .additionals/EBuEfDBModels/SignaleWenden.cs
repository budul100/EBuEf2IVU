using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class SignaleWenden
    {
        public int Id { get; set; }
        public int SignalId { get; set; }
        public int GegensignalId { get; set; }

        public virtual Signale Gegensignal { get; set; }
        public virtual Signale Signal { get; set; }
    }
}
