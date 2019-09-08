using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class Fahrzeuge
    {
        public Fahrzeuge()
        {
            Stoerungen = new HashSet<Stoerungen>();
        }

        public int Id { get; set; }
        public int Adresse { get; set; }
        public int Slot { get; set; }
        public DateTime Timestamp { get; set; }
        public int? Speed { get; set; }
        public sbyte? Dir { get; set; }
        public sbyte? F0 { get; set; }
        public string Zugtyp { get; set; }
        public int Zuglaenge { get; set; }
        public int? PrevSpeed { get; set; }
        public int Fzs { get; set; }
        public float Verzoegerung { get; set; }
        public string Zustand { get; set; }

        public virtual FahrzeugeDaten FahrzeugeDaten { get; set; }
        public virtual ICollection<Stoerungen> Stoerungen { get; set; }
    }
}
