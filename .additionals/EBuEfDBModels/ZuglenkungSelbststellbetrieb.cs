using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class ZuglenkungSelbststellbetrieb
    {
        public int Id { get; set; }
        public int SignalId { get; set; }
        public int FahrstrasseId { get; set; }

        public virtual Fahrstrassen Fahrstrasse { get; set; }
        public virtual Signale Signal { get; set; }
    }
}
