using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class ZuegeZuggattungen
    {
        public ZuegeZuggattungen()
        {
            FahrplanMindesthaltezeiten = new HashSet<FahrplanMindesthaltezeiten>();
            FahrplanZuege = new HashSet<FahrplanZuege>();
        }

        public int Id { get; set; }
        public string Zuggattung { get; set; }
        public string Bezeichnung { get; set; }
        public string Verkehrsart { get; set; }
        public string Fzs { get; set; }
        public string BildfahrplanFarbe { get; set; }

        public virtual ZuegeVerkehrsarten VerkehrsartNavigation { get; set; }
        public virtual ICollection<FahrplanMindesthaltezeiten> FahrplanMindesthaltezeiten { get; set; }
        public virtual ICollection<FahrplanZuege> FahrplanZuege { get; set; }
    }
}
