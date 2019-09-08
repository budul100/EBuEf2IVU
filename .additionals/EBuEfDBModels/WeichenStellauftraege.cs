using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class WeichenStellauftraege
    {
        public int Id { get; set; }
        public int InfraId { get; set; }
        public int SollDir { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
