using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrstrassenEinstellung
    {
        public int Id { get; set; }
        public int FahrstrasseId { get; set; }
        public int AufloeseId1 { get; set; }
        public int AufloeseId2 { get; set; }
        public int SignalId { get; set; }
        public string Fuem { get; set; }
        public string Zfm { get; set; }
        public int Status { get; set; }
        public int Unixtimestamp { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
