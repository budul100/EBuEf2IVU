using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrstrassenDwege
    {
        public int Id { get; set; }
        public int FahrstrasseId { get; set; }
        public int DwegId { get; set; }
        public int SignalbegriffId { get; set; }

        public virtual Dwege Dweg { get; set; }
        public virtual Fahrstrassen Fahrstrasse { get; set; }
        public virtual SignaleBegriffe Signalbegriff { get; set; }
    }
}
