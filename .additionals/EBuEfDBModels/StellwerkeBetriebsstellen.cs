using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class StellwerkeBetriebsstellen
    {
        public int Id { get; set; }
        public int StellwerkId { get; set; }
        public string BetriebsstelleKuerzel { get; set; }

        public virtual BetriebsstellenListe BetriebsstelleKuerzelNavigation { get; set; }
        public virtual Stellwerke Stellwerk { get; set; }
    }
}
