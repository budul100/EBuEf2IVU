using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class InfraZustand
    {
        public InfraZustand()
        {
            BahnuebergaengeElementeAufloesung = new HashSet<BahnuebergaengeElemente>();
            BahnuebergaengeElementeInfra = new HashSet<BahnuebergaengeElemente>();
            BetriebsstellenStatusElemente = new HashSet<BetriebsstellenStatusElemente>();
            BlockfelderBlockfeld = new HashSet<Blockfelder>();
            BlockfelderElemente = new HashSet<BlockfelderElemente>();
            BlockfelderErlaubnis = new HashSet<Blockfelder>();
            Dwege = new HashSet<Dwege>();
            DwegeElemente = new HashSet<DwegeElemente>();
            Erlaubnisfelder = new HashSet<Erlaubnisfelder>();
            FahrstrassenAnstoss = new HashSet<FahrstrassenAnstoss>();
            FahrstrassenElementeFreimeldeabschnitt = new HashSet<FahrstrassenElemente>();
            FahrstrassenElementeInfra = new HashSet<FahrstrassenElemente>();
            FahrstrassenFolgen = new HashSet<FahrstrassenFolgen>();
            GbtFma = new HashSet<GbtFma>();
            InfraDatenInfra = new HashSet<InfraDaten>();
            InfraDatenLinked = new HashSet<InfraDaten>();
            InfraDatenStoerung = new HashSet<InfraDaten>();
            InfraFestlegung = new HashSet<InfraFestlegung>();
            InverseFreimeldeabschnitt = new HashSet<InfraZustand>();
            Protokoll = new HashSet<Protokoll>();
            SignaleAnhalte = new HashSet<Signale>();
            SignaleElemente = new HashSet<SignaleElemente>();
            SignaleFreifahrt = new HashSet<Signale>();
            SignaleFreimelde = new HashSet<Signale>();
            SignaleFreimeldeId2Navigation = new HashSet<Signale>();
            SignaleHaltabschnitt = new HashSet<Signale>();
            SignaleHaltfall = new HashSet<Signale>();
            StellwerkeHandlungUnzulaessig = new HashSet<StellwerkeHandlungUnzulaessig>();
            StellwerkeMerkschilder = new HashSet<StellwerkeMerkschilder>();
            StellwerkeProtokoll = new HashSet<StellwerkeProtokoll>();
            StellwerkeZuordnung = new HashSet<StellwerkeZuordnung>();
            Stoerungen = new HashSet<Stoerungen>();
            WeichenAbhaengigkeit = new HashSet<WeichenAbhaengigkeit>();
            WeichenVorzugslage = new HashSet<WeichenVorzugslage>();
        }

        public int Id { get; set; }
        public int? Address { get; set; }
        public string Type { get; set; }
        public sbyte Dir { get; set; }
        public int Wrm { get; set; }
        public int WrmAktiv { get; set; }
        public int Stellzaehler { get; set; }
        public int Festlegung { get; set; }
        public int FestlegungBlock { get; set; }
        public int FestlegungRa { get; set; }
        public int? Laenge { get; set; }
        public string Betriebsstelle { get; set; }
        public int? SignalstandortId { get; set; }
        public int? FreimeldeabschnittId { get; set; }
        public int? WeichenabhaengigkeitId { get; set; }
        public string Kurzbezeichnung { get; set; }
        public string Bezeichnung { get; set; }
        public string Plan { get; set; }
        public string Blatt { get; set; }
        public int Lock { get; set; }
        public int Unixtimestamp { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual BetriebsstellenListe BetriebsstelleNavigation { get; set; }
        public virtual InfraZustand Freimeldeabschnitt { get; set; }
        public virtual Signale Signalstandort { get; set; }
        public virtual InfraTypen TypeNavigation { get; set; }
        public virtual ICollection<BahnuebergaengeElemente> BahnuebergaengeElementeAufloesung { get; set; }
        public virtual ICollection<BahnuebergaengeElemente> BahnuebergaengeElementeInfra { get; set; }
        public virtual ICollection<BetriebsstellenStatusElemente> BetriebsstellenStatusElemente { get; set; }
        public virtual ICollection<Blockfelder> BlockfelderBlockfeld { get; set; }
        public virtual ICollection<BlockfelderElemente> BlockfelderElemente { get; set; }
        public virtual ICollection<Blockfelder> BlockfelderErlaubnis { get; set; }
        public virtual ICollection<Dwege> Dwege { get; set; }
        public virtual ICollection<DwegeElemente> DwegeElemente { get; set; }
        public virtual ICollection<Erlaubnisfelder> Erlaubnisfelder { get; set; }
        public virtual ICollection<FahrstrassenAnstoss> FahrstrassenAnstoss { get; set; }
        public virtual ICollection<FahrstrassenElemente> FahrstrassenElementeFreimeldeabschnitt { get; set; }
        public virtual ICollection<FahrstrassenElemente> FahrstrassenElementeInfra { get; set; }
        public virtual ICollection<FahrstrassenFolgen> FahrstrassenFolgen { get; set; }
        public virtual ICollection<GbtFma> GbtFma { get; set; }
        public virtual ICollection<InfraDaten> InfraDatenInfra { get; set; }
        public virtual ICollection<InfraDaten> InfraDatenLinked { get; set; }
        public virtual ICollection<InfraDaten> InfraDatenStoerung { get; set; }
        public virtual ICollection<InfraFestlegung> InfraFestlegung { get; set; }
        public virtual ICollection<InfraZustand> InverseFreimeldeabschnitt { get; set; }
        public virtual ICollection<Protokoll> Protokoll { get; set; }
        public virtual ICollection<Signale> SignaleAnhalte { get; set; }
        public virtual ICollection<SignaleElemente> SignaleElemente { get; set; }
        public virtual ICollection<Signale> SignaleFreifahrt { get; set; }
        public virtual ICollection<Signale> SignaleFreimelde { get; set; }
        public virtual ICollection<Signale> SignaleFreimeldeId2Navigation { get; set; }
        public virtual ICollection<Signale> SignaleHaltabschnitt { get; set; }
        public virtual ICollection<Signale> SignaleHaltfall { get; set; }
        public virtual ICollection<StellwerkeHandlungUnzulaessig> StellwerkeHandlungUnzulaessig { get; set; }
        public virtual ICollection<StellwerkeMerkschilder> StellwerkeMerkschilder { get; set; }
        public virtual ICollection<StellwerkeProtokoll> StellwerkeProtokoll { get; set; }
        public virtual ICollection<StellwerkeZuordnung> StellwerkeZuordnung { get; set; }
        public virtual ICollection<Stoerungen> Stoerungen { get; set; }
        public virtual ICollection<WeichenAbhaengigkeit> WeichenAbhaengigkeit { get; set; }
        public virtual ICollection<WeichenVorzugslage> WeichenVorzugslage { get; set; }
    }
}
