using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class DwegeEinstellungElemente
    {
        public int Id { get; set; }
        public int InfraId { get; set; }
        public int DwegId { get; set; }
        public int? InfraDir { get; set; }
        public int SignalId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
