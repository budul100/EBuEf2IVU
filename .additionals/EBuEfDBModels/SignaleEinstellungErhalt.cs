using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class SignaleEinstellungErhalt
    {
        public int Id { get; set; }
        public int SignalId { get; set; }
        public int SignalbegriffId { get; set; }
        public int HaltabschnittId { get; set; }
        public int HaltfallId { get; set; }
        public int Signalfahrtstellung { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
