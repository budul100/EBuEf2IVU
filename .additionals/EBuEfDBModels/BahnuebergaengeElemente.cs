using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class BahnuebergaengeElemente
    {
        public int Id { get; set; }
        public int BahnuebergangId { get; set; }
        public int InfraId { get; set; }
        public int GrundstellungDir { get; set; }
        public int ZielDir { get; set; }
        public int? AufloesungId { get; set; }

        public virtual InfraZustand Aufloesung { get; set; }
        public virtual Bahnuebergaenge Bahnuebergang { get; set; }
        public virtual InfraZustand Infra { get; set; }
    }
}
