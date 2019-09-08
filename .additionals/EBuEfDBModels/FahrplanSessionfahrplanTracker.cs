using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrplanSessionfahrplanTracker
    {
        public int Id { get; set; }
        public int FahrplanSessionfahrplanId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
