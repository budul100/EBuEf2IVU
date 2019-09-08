using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrstrassenFolgen
    {
        public int Id { get; set; }
        public int Fahrstrasse1Id { get; set; }
        public int Fahrstrasse2Id { get; set; }
        public int AufloeseId { get; set; }
        public int StartsignalbegriffId { get; set; }
        public int ZielsignalbegriffId { get; set; }
        public int Standard { get; set; }

        public virtual InfraZustand Aufloese { get; set; }
        public virtual Fahrstrassen Fahrstrasse1 { get; set; }
        public virtual Fahrstrassen Fahrstrasse2 { get; set; }
        public virtual SignaleBegriffe Startsignalbegriff { get; set; }
        public virtual SignaleBegriffe Zielsignalbegriff { get; set; }
    }
}
