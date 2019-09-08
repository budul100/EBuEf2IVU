using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class Signale
    {
        public Signale()
        {
            BetriebsstellenStatusElemente = new HashSet<BetriebsstellenStatusElemente>();
            Dwege = new HashSet<Dwege>();
            FahrstrassenStartsignal = new HashSet<Fahrstrassen>();
            FahrstrassenZielsignal = new HashSet<Fahrstrassen>();
            InfraZustand = new HashSet<InfraZustand>();
            SignaleBegriffe = new HashSet<SignaleBegriffe>();
            SignaleWendenGegensignal = new HashSet<SignaleWenden>();
            SignaleWendenSignal = new HashSet<SignaleWenden>();
            SignaleZielpunkte = new HashSet<SignaleZielpunkte>();
            StellwerkeProtokoll = new HashSet<StellwerkeProtokoll>();
            StellwerkeZuordnung = new HashSet<StellwerkeZuordnung>();
        }

        public int Id { get; set; }
        public string Bezeichnung { get; set; }
        public string Signaltyp { get; set; }
        public float Standort { get; set; }
        public int? HaltabschnittId { get; set; }
        public int? FreimeldeId { get; set; }
        public int? FreimeldeId2 { get; set; }
        public int? AnhalteId { get; set; }
        public int? FreifahrtId { get; set; }
        public int? HaltbegriffId { get; set; }
        public int? WendenId { get; set; }
        public int? GbtId { get; set; }
        public int? HaltfallId { get; set; }
        public int Wirkrichtung { get; set; }
        public string Wirkart { get; set; }
        public string Betriebsstelle { get; set; }
        public string Fahrplanhalt { get; set; }
        public string Folgebetriebsstelle { get; set; }
        public int Zuglenkung { get; set; }
        public int ZlDefault { get; set; }
        public string BezeichnungAlt { get; set; }
        public int Lock { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual InfraZustand Anhalte { get; set; }
        public virtual BetriebsstellenListe BetriebsstelleNavigation { get; set; }
        public virtual BetriebsstellenListe FolgebetriebsstelleNavigation { get; set; }
        public virtual InfraZustand Freifahrt { get; set; }
        public virtual InfraZustand Freimelde { get; set; }
        public virtual InfraZustand FreimeldeId2Navigation { get; set; }
        public virtual Gbt Gbt { get; set; }
        public virtual InfraZustand Haltabschnitt { get; set; }
        public virtual SignaleBegriffe Haltbegriff { get; set; }
        public virtual InfraZustand Haltfall { get; set; }
        public virtual ZuglenkungSelbststellbetrieb ZuglenkungSelbststellbetrieb { get; set; }
        public virtual ICollection<BetriebsstellenStatusElemente> BetriebsstellenStatusElemente { get; set; }
        public virtual ICollection<Dwege> Dwege { get; set; }
        public virtual ICollection<Fahrstrassen> FahrstrassenStartsignal { get; set; }
        public virtual ICollection<Fahrstrassen> FahrstrassenZielsignal { get; set; }
        public virtual ICollection<InfraZustand> InfraZustand { get; set; }
        public virtual ICollection<SignaleBegriffe> SignaleBegriffe { get; set; }
        public virtual ICollection<SignaleWenden> SignaleWendenGegensignal { get; set; }
        public virtual ICollection<SignaleWenden> SignaleWendenSignal { get; set; }
        public virtual ICollection<SignaleZielpunkte> SignaleZielpunkte { get; set; }
        public virtual ICollection<StellwerkeProtokoll> StellwerkeProtokoll { get; set; }
        public virtual ICollection<StellwerkeZuordnung> StellwerkeZuordnung { get; set; }
    }
}
