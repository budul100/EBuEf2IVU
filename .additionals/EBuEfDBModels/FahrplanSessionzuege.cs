using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrplanSessionzuege
    {
        public int Id { get; set; }
        public int ZuggattungId { get; set; }
        public int Zugnummer { get; set; }
        public string Verkehrstage { get; set; }
        public string VerkehrstageBin { get; set; }
        public int Vmax { get; set; }
        public int VmaxIst { get; set; }
        public int Triebfahrzeug { get; set; }
        public int TriebfahrzeugIst { get; set; }
        public int? FahrzeugId { get; set; }
        public string Bremssystem { get; set; }
        public int Mbr { get; set; }
        public int Wendezug { get; set; }
        public int? UebergangVonZugId { get; set; }
        public int? UebergangNachZugId { get; set; }
        public string Bemerkungen { get; set; }
    }
}
