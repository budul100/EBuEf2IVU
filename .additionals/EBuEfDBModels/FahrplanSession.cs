using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrplanSession
    {
        public FahrplanSession()
        {
            Protokoll = new HashSet<Protokoll>();
            StellwerkeProtokoll = new HashSet<StellwerkeProtokoll>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Sessionkey { get; set; }
        public int RealStartzeit { get; set; }
        public int Timeshift { get; set; }
        public int? SimStartzeit { get; set; }
        public int? SimEndzeit { get; set; }
        public string SimWochentag { get; set; }
        public int SimPausezeit { get; set; }
        public float Skalierung { get; set; }
        public int FahrplanId { get; set; }
        public sbyte ZnAutowechsel { get; set; }
        public int FzsFahrplanbasiert { get; set; }
        public string BetriebsstelleXap { get; set; }
        public string BetriebsstelleXlg { get; set; }
        public string BetriebsstelleXwf { get; set; }
        public sbyte Status { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual Fahrplan Fahrplan { get; set; }
        public virtual ICollection<Protokoll> Protokoll { get; set; }
        public virtual ICollection<StellwerkeProtokoll> StellwerkeProtokoll { get; set; }
    }
}
