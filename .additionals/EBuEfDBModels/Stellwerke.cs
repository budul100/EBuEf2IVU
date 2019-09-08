using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class Stellwerke
    {
        public Stellwerke()
        {
            StellwerkeBereiche = new HashSet<StellwerkeBereiche>();
            StellwerkeBetriebsstellen = new HashSet<StellwerkeBetriebsstellen>();
            StellwerkeHandlungUnzulaessig = new HashSet<StellwerkeHandlungUnzulaessig>();
            UserStellwerke = new HashSet<UserStellwerke>();
        }

        public int Id { get; set; }
        public string Stwname { get; set; }
        public string Kurzname { get; set; }
        public string Betriebsstelle { get; set; }
        public string SvgFile { get; set; }
        public int SvgHeight { get; set; }
        public int SvgWidth { get; set; }
        public sbyte Decoderplan { get; set; }
        public sbyte Idplan { get; set; }
        public sbyte Lupe { get; set; }
        public sbyte Berue { get; set; }
        public sbyte Zwl { get; set; }
        public sbyte Streckenspiegel { get; set; }
        public sbyte Zn { get; set; }
        public int? HasLenkplan { get; set; }
        public string ZwlLink { get; set; }
        public string Special { get; set; }

        public virtual BetriebsstellenListe BetriebsstelleNavigation { get; set; }
        public virtual ICollection<StellwerkeBereiche> StellwerkeBereiche { get; set; }
        public virtual ICollection<StellwerkeBetriebsstellen> StellwerkeBetriebsstellen { get; set; }
        public virtual ICollection<StellwerkeHandlungUnzulaessig> StellwerkeHandlungUnzulaessig { get; set; }
        public virtual ICollection<UserStellwerke> UserStellwerke { get; set; }
    }
}
