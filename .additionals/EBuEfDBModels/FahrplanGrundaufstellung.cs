using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrplanGrundaufstellung
    {
        public int Id { get; set; }
        public int Zugnummer { get; set; }
        public int ZugId { get; set; }
        public int UmlaufId { get; set; }
        public int FahrzeugAdresse { get; set; }
        public string Betriebsstelle { get; set; }
        public int GbtId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
