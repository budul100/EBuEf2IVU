using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class SignaleStellauftraege
    {
        public int Id { get; set; }
        public int SignalbegriffId { get; set; }
        public int FahrstrasseId { get; set; }
        public int IstStartsignal { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
