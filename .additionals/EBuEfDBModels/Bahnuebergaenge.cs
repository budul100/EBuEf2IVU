using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class Bahnuebergaenge
    {
        public Bahnuebergaenge()
        {
            BahnuebergaengeElemente = new HashSet<BahnuebergaengeElemente>();
        }

        public int Id { get; set; }
        public string Bezeichnung { get; set; }
        public string Betriebsstelle { get; set; }
        public int Sicherung { get; set; }
        public string Freimeldeart { get; set; }
        public int Lock { get; set; }
        public sbyte Dauereinschaltung { get; set; }
        public string Stoerung { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual BetriebsstellenListe BetriebsstelleNavigation { get; set; }
        public virtual ICollection<BahnuebergaengeElemente> BahnuebergaengeElemente { get; set; }
    }
}
