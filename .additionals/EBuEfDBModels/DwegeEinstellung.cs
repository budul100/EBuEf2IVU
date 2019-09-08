using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class DwegeEinstellung
    {
        public int Id { get; set; }
        public int FahrstrasseId { get; set; }
        public int DwegId { get; set; }
        public int AufloeseId { get; set; }
        public int AufloeseVerzoegerung { get; set; }
        public int SignalId { get; set; }
        public int Dfm { get; set; }
        public int? IsDurchfahrt { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
