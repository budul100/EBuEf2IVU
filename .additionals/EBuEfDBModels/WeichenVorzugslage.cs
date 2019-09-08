using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class WeichenVorzugslage
    {
        public int Id { get; set; }
        public int InfraId { get; set; }
        public int Dir { get; set; }

        public virtual InfraZustand Infra { get; set; }
    }
}
