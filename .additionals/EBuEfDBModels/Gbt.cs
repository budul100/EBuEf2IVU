using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class Gbt
    {
        public Gbt()
        {
            BetriebsstellenStatusElemente = new HashSet<BetriebsstellenStatusElemente>();
            FahrstrassenAnstoss = new HashSet<FahrstrassenAnstoss>();
            FahrstrassenStartfeld = new HashSet<Fahrstrassen>();
            FahrstrassenZielfeld = new HashSet<Fahrstrassen>();
            GbtAnbietefelderAnbietefeld = new HashSet<GbtAnbietefelder>();
            GbtAnbietefelderGbt = new HashSet<GbtAnbietefelder>();
            GbtBetriebsstellenStartGbt = new HashSet<GbtBetriebsstellen>();
            GbtBetriebsstellenZielGbt = new HashSet<GbtBetriebsstellen>();
            GbtFma = new HashSet<GbtFma>();
            Protokoll = new HashSet<Protokoll>();
            Signale = new HashSet<Signale>();
            StellwerkeProtokoll = new HashSet<StellwerkeProtokoll>();
        }

        public int Id { get; set; }
        public string Zugnummer { get; set; }
        public int Decoder { get; set; }
        public sbyte? Visible { get; set; }
        public int IsAnbietefeld { get; set; }
        public int IsVirtuell { get; set; }
        public int Status { get; set; }
        public string Vormeldung0 { get; set; }
        public string Vormeldung1 { get; set; }
        public string Betriebsstelle { get; set; }
        public int Gleis { get; set; }
        public string Kurzbezeichnung { get; set; }
        public int FzgFahrstufe { get; set; }
        public string Bearbeiter { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual BetriebsstellenListe BetriebsstelleNavigation { get; set; }
        public virtual ICollection<BetriebsstellenStatusElemente> BetriebsstellenStatusElemente { get; set; }
        public virtual ICollection<FahrstrassenAnstoss> FahrstrassenAnstoss { get; set; }
        public virtual ICollection<Fahrstrassen> FahrstrassenStartfeld { get; set; }
        public virtual ICollection<Fahrstrassen> FahrstrassenZielfeld { get; set; }
        public virtual ICollection<GbtAnbietefelder> GbtAnbietefelderAnbietefeld { get; set; }
        public virtual ICollection<GbtAnbietefelder> GbtAnbietefelderGbt { get; set; }
        public virtual ICollection<GbtBetriebsstellen> GbtBetriebsstellenStartGbt { get; set; }
        public virtual ICollection<GbtBetriebsstellen> GbtBetriebsstellenZielGbt { get; set; }
        public virtual ICollection<GbtFma> GbtFma { get; set; }
        public virtual ICollection<Protokoll> Protokoll { get; set; }
        public virtual ICollection<Signale> Signale { get; set; }
        public virtual ICollection<StellwerkeProtokoll> StellwerkeProtokoll { get; set; }
    }
}
