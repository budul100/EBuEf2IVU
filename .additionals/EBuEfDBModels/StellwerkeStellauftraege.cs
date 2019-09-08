using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class StellwerkeStellauftraege
    {
        public int Id { get; set; }
        public int UzId { get; set; }
        public int? InfraId { get; set; }
        public int? ZielDir { get; set; }
        public int? DwegId { get; set; }
        public int? FahrstrasseId { get; set; }
    }
}
