using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class Fahrplan
    {
        public Fahrplan()
        {
            FahrplanSession = new HashSet<FahrplanSession>();
            FahrplanZuege = new HashSet<FahrplanZuege>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Bezeichnung { get; set; }
        public string Autor { get; set; }
        public DateTime GueltigAb { get; set; }
        public DateTime GueltigBis { get; set; }
        public string Aktiv { get; set; }
        public string Typ { get; set; }
        public string Importquelle { get; set; }

        public virtual ICollection<FahrplanSession> FahrplanSession { get; set; }
        public virtual ICollection<FahrplanZuege> FahrplanZuege { get; set; }
    }
}
