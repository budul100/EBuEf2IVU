using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class UserRechteliste
    {
        public UserRechteliste()
        {
            UserZugriffsrechte = new HashSet<UserZugriffsrechte>();
        }

        public int Id { get; set; }
        public string Recht { get; set; }
        public string Beschreibung { get; set; }

        public virtual ICollection<UserZugriffsrechte> UserZugriffsrechte { get; set; }
    }
}
