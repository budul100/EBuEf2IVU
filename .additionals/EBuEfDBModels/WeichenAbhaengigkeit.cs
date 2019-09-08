using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class WeichenAbhaengigkeit
    {
        public int Id { get; set; }
        public int AbhaengigkeitId { get; set; }
        public int WeicheId { get; set; }
        public int WeicheDir { get; set; }

        public virtual InfraZustand Weiche { get; set; }
    }
}
