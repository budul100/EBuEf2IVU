using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrzeugeGruppen
    {
        public FahrzeugeGruppen()
        {
            FahrzeugeBaureiheGruppe = new HashSet<FahrzeugeBaureiheGruppe>();
            FahrzeugeBaureihen = new HashSet<FahrzeugeBaureihen>();
        }

        public int Id { get; set; }
        public string Bezeichnung { get; set; }

        public virtual ICollection<FahrzeugeBaureiheGruppe> FahrzeugeBaureiheGruppe { get; set; }
        public virtual ICollection<FahrzeugeBaureihen> FahrzeugeBaureihen { get; set; }
    }
}
