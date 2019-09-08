using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrstrassenElementtypen
    {
        public FahrstrassenElementtypen()
        {
            DwegeElemente = new HashSet<DwegeElemente>();
            FahrstrassenElemente = new HashSet<FahrstrassenElemente>();
        }

        public int Id { get; set; }
        public int Wert { get; set; }
        public string Beschreibung { get; set; }

        public virtual ICollection<DwegeElemente> DwegeElemente { get; set; }
        public virtual ICollection<FahrstrassenElemente> FahrstrassenElemente { get; set; }
    }
}
