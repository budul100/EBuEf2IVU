using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class StellwerkeUz
    {
        public StellwerkeUz()
        {
            StellwerkeBereiche = new HashSet<StellwerkeBereiche>();
            StellwerkeMerkspeicher = new HashSet<StellwerkeMerkspeicher>();
            StellwerkeZuordnung = new HashSet<StellwerkeZuordnung>();
        }

        public int Id { get; set; }
        public string Betriebsstelle { get; set; }
        public string Uzname { get; set; }
        public string Wlk { get; set; }
        public string Tanaspannung { get; set; }
        public string Elementbezeichnungen { get; set; }
        public int Zaehlwerk { get; set; }
        public int? AktivesStellwerkId { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual BetriebsstellenListe BetriebsstelleNavigation { get; set; }
        public virtual ICollection<StellwerkeBereiche> StellwerkeBereiche { get; set; }
        public virtual ICollection<StellwerkeMerkspeicher> StellwerkeMerkspeicher { get; set; }
        public virtual ICollection<StellwerkeZuordnung> StellwerkeZuordnung { get; set; }
    }
}
