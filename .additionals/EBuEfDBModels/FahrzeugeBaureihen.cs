using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrzeugeBaureihen
    {
        public FahrzeugeBaureihen()
        {
            FahrplanZuege = new HashSet<FahrplanZuege>();
            FahrzeugeBaureiheGruppe = new HashSet<FahrzeugeBaureiheGruppe>();
            FahrzeugeDaten = new HashSet<FahrzeugeDaten>();
        }

        public int Id { get; set; }
        public int Nummer { get; set; }
        public string Bezeichnung { get; set; }
        public int Vmax { get; set; }
        public int? Fahrzeuggruppe { get; set; }
        public int Laenge { get; set; }
        public string Traktion { get; set; }

        public virtual FahrzeugeGruppen FahrzeuggruppeNavigation { get; set; }
        public virtual ICollection<FahrplanZuege> FahrplanZuege { get; set; }
        public virtual ICollection<FahrzeugeBaureiheGruppe> FahrzeugeBaureiheGruppe { get; set; }
        public virtual ICollection<FahrzeugeDaten> FahrzeugeDaten { get; set; }
    }
}
