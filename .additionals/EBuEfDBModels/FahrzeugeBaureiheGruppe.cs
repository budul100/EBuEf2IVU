using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrzeugeBaureiheGruppe
    {
        public int Id { get; set; }
        public int BaureiheId { get; set; }
        public int GruppeId { get; set; }

        public virtual FahrzeugeBaureihen Baureihe { get; set; }
        public virtual FahrzeugeGruppen Gruppe { get; set; }
    }
}
