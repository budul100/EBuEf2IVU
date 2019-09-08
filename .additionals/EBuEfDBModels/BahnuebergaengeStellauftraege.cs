using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class BahnuebergaengeStellauftraege
    {
        public int Id { get; set; }
        public int BahnuebergangId { get; set; }
        public int InfraId { get; set; }
        public int ZielDir { get; set; }
        public int Zeitpunkt { get; set; }
        public int Ausfuehrung { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
