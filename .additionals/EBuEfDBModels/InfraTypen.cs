using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class InfraTypen
    {
        public InfraTypen()
        {
            InfraZustand = new HashSet<InfraZustand>();
        }

        public int Id { get; set; }
        public string Type { get; set; }

        public virtual ICollection<InfraZustand> InfraZustand { get; set; }
    }
}
