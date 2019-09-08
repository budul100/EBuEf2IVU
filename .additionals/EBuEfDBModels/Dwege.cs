using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class Dwege
    {
        public Dwege()
        {
            DwegeElemente = new HashSet<DwegeElemente>();
            Fahrstrassen = new HashSet<Fahrstrassen>();
            FahrstrassenDwege = new HashSet<FahrstrassenDwege>();
        }

        public int Id { get; set; }
        public string Bezeichnung { get; set; }
        public string Betriebsstelle { get; set; }
        public int SignalId { get; set; }
        public int? AufloeseId { get; set; }
        public int AufloeseVerzoegerung { get; set; }
        public string Ende { get; set; }
        public string Vmax { get; set; }
        public int LaengeSoll { get; set; }
        public int LaengeIst { get; set; }
        public float Neigung { get; set; }

        public virtual InfraZustand Aufloese { get; set; }
        public virtual BetriebsstellenListe BetriebsstelleNavigation { get; set; }
        public virtual Signale Signal { get; set; }
        public virtual ICollection<DwegeElemente> DwegeElemente { get; set; }
        public virtual ICollection<Fahrstrassen> Fahrstrassen { get; set; }
        public virtual ICollection<FahrstrassenDwege> FahrstrassenDwege { get; set; }
    }
}
