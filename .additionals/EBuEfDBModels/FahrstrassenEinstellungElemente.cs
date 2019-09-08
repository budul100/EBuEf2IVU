using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrstrassenEinstellungElemente
    {
        public int Id { get; set; }
        public int InfraId { get; set; }
        public int FreimeldeabschnittId { get; set; }
        public int FahrstrasseId { get; set; }
        public string EinstellungId { get; set; }
        public int Unixtimestamp { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
