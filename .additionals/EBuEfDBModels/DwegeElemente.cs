using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class DwegeElemente
    {
        public int Id { get; set; }
        public int DwegId { get; set; }
        public int InfraId { get; set; }
        public int Dir { get; set; }
        public int Reihenfolge { get; set; }

        public virtual FahrstrassenElementtypen DirNavigation { get; set; }
        public virtual Dwege Dweg { get; set; }
        public virtual InfraZustand Infra { get; set; }
    }
}
