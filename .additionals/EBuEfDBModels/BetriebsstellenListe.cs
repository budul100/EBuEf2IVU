using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class BetriebsstellenListe
    {
        public BetriebsstellenListe()
        {
            Bahnuebergaenge = new HashSet<Bahnuebergaenge>();
            BetriebsstellenErsetzungBezugNavigation = new HashSet<BetriebsstellenErsetzung>();
            BetriebsstellenErsetzungErsatzNavigation = new HashSet<BetriebsstellenErsetzung>();
            BetriebsstellenErsetzungUrsprung2Navigation = new HashSet<BetriebsstellenErsetzung>();
            BetriebsstellenErsetzungUrsprung3Navigation = new HashSet<BetriebsstellenErsetzung>();
            BetriebsstellenErsetzungUrsprungNavigation = new HashSet<BetriebsstellenErsetzung>();
            BetriebsstellenFahrtrichtungBezugNavigation = new HashSet<BetriebsstellenFahrtrichtung>();
            BetriebsstellenFahrtrichtungNachbarNavigation = new HashSet<BetriebsstellenFahrtrichtung>();
            BetriebsstellenHinweiseBezugNavigation = new HashSet<BetriebsstellenHinweise>();
            BetriebsstellenHinweiseNachNavigation = new HashSet<BetriebsstellenHinweise>();
            BetriebsstellenHinweiseVonNavigation = new HashSet<BetriebsstellenHinweise>();
            BetriebsstellenStatus = new HashSet<BetriebsstellenStatus>();
            Dwege = new HashSet<Dwege>();
            ErlaubnisfelderBetriebsstelle0Navigation = new HashSet<Erlaubnisfelder>();
            ErlaubnisfelderBetriebsstelle1Navigation = new HashSet<Erlaubnisfelder>();
            FahrplanMindesthaltezeiten = new HashSet<FahrplanMindesthaltezeiten>();
            Fahrstrassen = new HashSet<Fahrstrassen>();
            Gbt = new HashSet<Gbt>();
            GbtBetriebsstellenStartBetriebsstelleNavigation = new HashSet<GbtBetriebsstellen>();
            GbtBetriebsstellenZielBetriebsstelle2Navigation = new HashSet<GbtBetriebsstellen>();
            GbtBetriebsstellenZielBetriebsstelleNavigation = new HashSet<GbtBetriebsstellen>();
            InfraZustand = new HashSet<InfraZustand>();
            InverseKuerzelGleisNavigation = new HashSet<BetriebsstellenListe>();
            InverseParentKuerzelNavigation = new HashSet<BetriebsstellenListe>();
            SignaleBetriebsstelleNavigation = new HashSet<Signale>();
            SignaleFolgebetriebsstelleNavigation = new HashSet<Signale>();
            Stellwerke = new HashSet<Stellwerke>();
            StellwerkeBetriebsstellen = new HashSet<StellwerkeBetriebsstellen>();
            StellwerkeProtokoll = new HashSet<StellwerkeProtokoll>();
            StellwerkeUz = new HashSet<StellwerkeUz>();
            Stoerungen = new HashSet<Stoerungen>();
            StreckenAbschnitteBetriebsstelle0Navigation = new HashSet<StreckenAbschnitte>();
            StreckenAbschnitteBetriebsstelle1Navigation = new HashSet<StreckenAbschnitte>();
        }

        public int Id { get; set; }
        public string Kuerzel { get; set; }
        public string ParentKuerzel { get; set; }
        public string Name { get; set; }
        public int Nummer { get; set; }
        public string Art { get; set; }
        public string Bstart { get; set; }
        public int? Wirkrichtung { get; set; }
        public int Zl { get; set; }
        public string KuerzelGleis { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        public virtual BetriebsstellenArt ArtNavigation { get; set; }
        public virtual BetriebsstellenListe KuerzelGleisNavigation { get; set; }
        public virtual BetriebsstellenListe ParentKuerzelNavigation { get; set; }
        public virtual ICollection<Bahnuebergaenge> Bahnuebergaenge { get; set; }
        public virtual ICollection<BetriebsstellenErsetzung> BetriebsstellenErsetzungBezugNavigation { get; set; }
        public virtual ICollection<BetriebsstellenErsetzung> BetriebsstellenErsetzungErsatzNavigation { get; set; }
        public virtual ICollection<BetriebsstellenErsetzung> BetriebsstellenErsetzungUrsprung2Navigation { get; set; }
        public virtual ICollection<BetriebsstellenErsetzung> BetriebsstellenErsetzungUrsprung3Navigation { get; set; }
        public virtual ICollection<BetriebsstellenErsetzung> BetriebsstellenErsetzungUrsprungNavigation { get; set; }
        public virtual ICollection<BetriebsstellenFahrtrichtung> BetriebsstellenFahrtrichtungBezugNavigation { get; set; }
        public virtual ICollection<BetriebsstellenFahrtrichtung> BetriebsstellenFahrtrichtungNachbarNavigation { get; set; }
        public virtual ICollection<BetriebsstellenHinweise> BetriebsstellenHinweiseBezugNavigation { get; set; }
        public virtual ICollection<BetriebsstellenHinweise> BetriebsstellenHinweiseNachNavigation { get; set; }
        public virtual ICollection<BetriebsstellenHinweise> BetriebsstellenHinweiseVonNavigation { get; set; }
        public virtual ICollection<BetriebsstellenStatus> BetriebsstellenStatus { get; set; }
        public virtual ICollection<Dwege> Dwege { get; set; }
        public virtual ICollection<Erlaubnisfelder> ErlaubnisfelderBetriebsstelle0Navigation { get; set; }
        public virtual ICollection<Erlaubnisfelder> ErlaubnisfelderBetriebsstelle1Navigation { get; set; }
        public virtual ICollection<FahrplanMindesthaltezeiten> FahrplanMindesthaltezeiten { get; set; }
        public virtual ICollection<Fahrstrassen> Fahrstrassen { get; set; }
        public virtual ICollection<Gbt> Gbt { get; set; }
        public virtual ICollection<GbtBetriebsstellen> GbtBetriebsstellenStartBetriebsstelleNavigation { get; set; }
        public virtual ICollection<GbtBetriebsstellen> GbtBetriebsstellenZielBetriebsstelle2Navigation { get; set; }
        public virtual ICollection<GbtBetriebsstellen> GbtBetriebsstellenZielBetriebsstelleNavigation { get; set; }
        public virtual ICollection<InfraZustand> InfraZustand { get; set; }
        public virtual ICollection<BetriebsstellenListe> InverseKuerzelGleisNavigation { get; set; }
        public virtual ICollection<BetriebsstellenListe> InverseParentKuerzelNavigation { get; set; }
        public virtual ICollection<Signale> SignaleBetriebsstelleNavigation { get; set; }
        public virtual ICollection<Signale> SignaleFolgebetriebsstelleNavigation { get; set; }
        public virtual ICollection<Stellwerke> Stellwerke { get; set; }
        public virtual ICollection<StellwerkeBetriebsstellen> StellwerkeBetriebsstellen { get; set; }
        public virtual ICollection<StellwerkeProtokoll> StellwerkeProtokoll { get; set; }
        public virtual ICollection<StellwerkeUz> StellwerkeUz { get; set; }
        public virtual ICollection<Stoerungen> Stoerungen { get; set; }
        public virtual ICollection<StreckenAbschnitte> StreckenAbschnitteBetriebsstelle0Navigation { get; set; }
        public virtual ICollection<StreckenAbschnitte> StreckenAbschnitteBetriebsstelle1Navigation { get; set; }
    }
}
