using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class Fma
    {
        public int Id { get; set; }
        public int FmaId { get; set; }
        public int Marco { get; set; }
        public int Type { get; set; }
        public int DecoderAdresse { get; set; }
        public string Bezeichnung { get; set; }
        public int Laenge { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual GbtFma GbtFma { get; set; }
    }
}
