using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrstrassenElemente
    {
        public int Id { get; set; }
        public int FahrstrassenId { get; set; }
        public int? InfraId { get; set; }
        public int Dir { get; set; }
        public int? SignalbegriffId { get; set; }
        public int Reihenfolge { get; set; }
        public int? FreimeldeabschnittId { get; set; }

        public virtual FahrstrassenElementtypen DirNavigation { get; set; }
        public virtual Fahrstrassen Fahrstrassen { get; set; }
        public virtual InfraZustand Freimeldeabschnitt { get; set; }
        public virtual InfraZustand Infra { get; set; }
        public virtual SignaleBegriffe Signalbegriff { get; set; }
    }
}
