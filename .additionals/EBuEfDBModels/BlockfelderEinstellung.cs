using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class BlockfelderEinstellung
    {
        public int Id { get; set; }
        public int FahrstrasseId { get; set; }
        public int BlockId1 { get; set; }
        public int BlockId2 { get; set; }
        public int SignalId { get; set; }
        public int VorblockfeldId { get; set; }
        public int RueckblockfeldId { get; set; }
        public int StreckenwiederholungssperreId { get; set; }
        public string Bluem { get; set; }
        public string Raeumungsmelder { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
