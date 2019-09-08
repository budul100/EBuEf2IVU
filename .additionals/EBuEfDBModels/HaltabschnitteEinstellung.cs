using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class HaltabschnitteEinstellung
    {
        public int Id { get; set; }
        public int FahrstrasseId { get; set; }
        public int HaltabschnittId { get; set; }
        public int FreimeldeabschnittId { get; set; }
        public int Unixtimestamp { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
