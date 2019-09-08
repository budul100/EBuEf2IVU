using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class SignaleEinstellung
    {
        public int Id { get; set; }
        public int FahrstrasseId { get; set; }
        public int SignalId { get; set; }
        public int ZielsignalbegriffId { get; set; }
        public int AufloeseId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
