using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class StellwerkeProtokoll
    {
        public int Id { get; set; }
        public string Betriebsstelle { get; set; }
        public int? Zaehlwerk { get; set; }
        public string Handlung { get; set; }
        public int? InfraId { get; set; }
        public int? SignalId { get; set; }
        public int? FahrstrasseId { get; set; }
        public int? GbtId { get; set; }
        public string Merktext { get; set; }
        public string Bediener { get; set; }
        public int? Zugnummer { get; set; }
        public string Kommentar { get; set; }
        public int? FahrplansessionId { get; set; }
        public DateTime? TimestampSession { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual BetriebsstellenListe BetriebsstelleNavigation { get; set; }
        public virtual FahrplanSession Fahrplansession { get; set; }
        public virtual Fahrstrassen Fahrstrasse { get; set; }
        public virtual Gbt Gbt { get; set; }
        public virtual InfraZustand Infra { get; set; }
        public virtual Signale Signal { get; set; }
    }
}
