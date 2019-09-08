using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class Stoerungen
    {
        public int Id { get; set; }
        public string Art { get; set; }
        public string Inhalt { get; set; }
        public int? FahrzeugId { get; set; }
        public int? InfraId { get; set; }
        public string Betriebsstelle { get; set; }
        public string Erfasser { get; set; }
        public int StatusMail { get; set; }
        public string Status { get; set; }
        public int Eintragzeit { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual BetriebsstellenListe BetriebsstelleNavigation { get; set; }
        public virtual Fahrzeuge Fahrzeug { get; set; }
        public virtual InfraZustand Infra { get; set; }
    }
}
