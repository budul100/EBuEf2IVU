using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrzeugeSteuerung
    {
        public int Id { get; set; }
        public int? FahrzeugId { get; set; }
        public int Ausfuehrung { get; set; }
        public string Geschwindigkeit { get; set; }
        public int Dir { get; set; }
        public int Signalstandortid { get; set; }
        public int Zugnummer { get; set; }
        public string Aktion { get; set; }
        public int V0 { get; set; }
        public int Vziel { get; set; }
        public int FreifahrtId { get; set; }
        public int Wenden { get; set; }
        public string Bemerkung { get; set; }
        public int Eintragzeit { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
