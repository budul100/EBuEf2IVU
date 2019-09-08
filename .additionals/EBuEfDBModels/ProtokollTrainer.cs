using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class ProtokollTrainer
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Nachricht { get; set; }
        public int Timestamp { get; set; }
    }
}
