using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrzeugeDaten
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Evu { get; set; }
        public int? Baureihe { get; set; }
        public DateTime Timestamp { get; set; }
        public int Railcom { get; set; }
        public float Radsatzmasse { get; set; }
        public string Decodertyp { get; set; }

        public virtual FahrzeugeBaureihen BaureiheNavigation { get; set; }
        public virtual Fahrzeuge IdNavigation { get; set; }
    }
}
