using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class Protokoll
    {
        public int Id { get; set; }
        public int Zugnummer { get; set; }
        public int Feld { get; set; }
        public DateTime Timestamp { get; set; }
        public string Source { get; set; }
        public int? Signal { get; set; }
        public int? SessionId { get; set; }
        public string IstBetriebsstelle { get; set; }
        public int IstVerspaetung { get; set; }
        public string IstArt { get; set; }
        public string Bearbeiter { get; set; }

        public virtual Gbt FeldNavigation { get; set; }
        public virtual FahrplanSession Session { get; set; }
        public virtual InfraZustand SignalNavigation { get; set; }
    }
}
