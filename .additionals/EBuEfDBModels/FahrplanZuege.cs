using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrplanZuege
    {
        public FahrplanZuege()
        {
            InverseUebergangNachZug = new HashSet<FahrplanZuege>();
            InverseUebergangVonZug = new HashSet<FahrplanZuege>();
        }

        public int Id { get; set; }
        public int ZuggattungId { get; set; }
        public int Zugnummer { get; set; }
        public string Verkehrstage { get; set; }
        public string VerkehrstageBin { get; set; }
        public int Vmax { get; set; }
        public int Triebfahrzeug { get; set; }
        public string Bremssystem { get; set; }
        public int Mbr { get; set; }
        public string Wendezug { get; set; }
        public int? UebergangVonZugId { get; set; }
        public int? UebergangNachZugId { get; set; }
        public string Bemerkungen { get; set; }
        public int FahrplanversionId { get; set; }

        public virtual Fahrplan Fahrplanversion { get; set; }
        public virtual FahrzeugeBaureihen TriebfahrzeugNavigation { get; set; }
        public virtual FahrplanZuege UebergangNachZug { get; set; }
        public virtual FahrplanZuege UebergangVonZug { get; set; }
        public virtual ZuegeZuggattungen Zuggattung { get; set; }
        public virtual ICollection<FahrplanZuege> InverseUebergangNachZug { get; set; }
        public virtual ICollection<FahrplanZuege> InverseUebergangVonZug { get; set; }
    }
}
