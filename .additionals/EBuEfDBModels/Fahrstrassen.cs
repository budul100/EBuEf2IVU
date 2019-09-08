using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class Fahrstrassen
    {
        public Fahrstrassen()
        {
            BetriebsstellenStatusElemente = new HashSet<BetriebsstellenStatusElemente>();
            FahrstrassenAnstoss = new HashSet<FahrstrassenAnstoss>();
            FahrstrassenDwege = new HashSet<FahrstrassenDwege>();
            FahrstrassenElemente = new HashSet<FahrstrassenElemente>();
            FahrstrassenFolgenFahrstrasse1 = new HashSet<FahrstrassenFolgen>();
            FahrstrassenFolgenFahrstrasse2 = new HashSet<FahrstrassenFolgen>();
            StellwerkeProtokoll = new HashSet<StellwerkeProtokoll>();
            ZuglenkungSelbststellbetrieb = new HashSet<ZuglenkungSelbststellbetrieb>();
        }

        public int Id { get; set; }
        public string Betriebsstelle { get; set; }
        public string Art { get; set; }
        public string Kurzbezeichnung { get; set; }
        public int? StartfeldId { get; set; }
        public int? ZielfeldId { get; set; }
        public int StartsignalId { get; set; }
        public int? StartsignalBegriffId { get; set; }
        public int? ZielsignalId { get; set; }
        public string ZielBezeichnung { get; set; }
        public string UmfahrBezeichnung { get; set; }
        public int? RegeldwegId { get; set; }
        public int F { get; set; }
        public int Fahrtrichtung { get; set; }
        public int ZlAnstoss1 { get; set; }
        public int ZlAnstoss1GbtId { get; set; }
        public int ZlAnstoss2 { get; set; }
        public int ZlAnstoss2GbtId { get; set; }
        public int ZlPrioritaet { get; set; }

        public virtual BetriebsstellenListe BetriebsstelleNavigation { get; set; }
        public virtual Dwege Regeldweg { get; set; }
        public virtual Gbt Startfeld { get; set; }
        public virtual Signale Startsignal { get; set; }
        public virtual SignaleBegriffe StartsignalBegriff { get; set; }
        public virtual Gbt Zielfeld { get; set; }
        public virtual Signale Zielsignal { get; set; }
        public virtual ICollection<BetriebsstellenStatusElemente> BetriebsstellenStatusElemente { get; set; }
        public virtual ICollection<FahrstrassenAnstoss> FahrstrassenAnstoss { get; set; }
        public virtual ICollection<FahrstrassenDwege> FahrstrassenDwege { get; set; }
        public virtual ICollection<FahrstrassenElemente> FahrstrassenElemente { get; set; }
        public virtual ICollection<FahrstrassenFolgen> FahrstrassenFolgenFahrstrasse1 { get; set; }
        public virtual ICollection<FahrstrassenFolgen> FahrstrassenFolgenFahrstrasse2 { get; set; }
        public virtual ICollection<StellwerkeProtokoll> StellwerkeProtokoll { get; set; }
        public virtual ICollection<ZuglenkungSelbststellbetrieb> ZuglenkungSelbststellbetrieb { get; set; }
    }
}
