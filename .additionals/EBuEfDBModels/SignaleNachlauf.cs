using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class SignaleNachlauf
    {
        public int Id { get; set; }
        public int SignalId { get; set; }
        public int SignalbegriffId { get; set; }
        public int FahrstrasseId { get; set; }
        public string Nachlaufart { get; set; }
        public int InfraId { get; set; }
        public int InfraDir { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
