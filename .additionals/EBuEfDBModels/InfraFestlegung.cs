using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class InfraFestlegung
    {
        public int Id { get; set; }
        public int InfraId { get; set; }
        public sbyte? Festlegung { get; set; }
        public int? Sperrung { get; set; }
        public string Art { get; set; }
        public string Urheber { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual InfraZustand Infra { get; set; }
    }
}
