using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrstrassenAnstoss
    {
        public int Id { get; set; }
        public int FahrstrasseId { get; set; }
        public int InfraId { get; set; }
        public int GbtId { get; set; }
        public int Verzoegerung { get; set; }

        public virtual Fahrstrassen Fahrstrasse { get; set; }
        public virtual Gbt Gbt { get; set; }
        public virtual InfraZustand Infra { get; set; }
    }
}
