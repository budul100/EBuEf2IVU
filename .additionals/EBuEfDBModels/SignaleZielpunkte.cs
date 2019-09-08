using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class SignaleZielpunkte
    {
        public int Id { get; set; }
        public int SignalId { get; set; }
        public string Zugtyp { get; set; }
        public int Zuglaenge { get; set; }
        public int Entfernung { get; set; }

        public virtual Signale Signal { get; set; }
    }
}
