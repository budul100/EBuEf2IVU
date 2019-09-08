using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class User
    {
        public User()
        {
            UserZugriffsrechte = new HashSet<UserZugriffsrechte>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Betriebsstelle { get; set; }
        public string Ipbereich { get; set; }

        public virtual ICollection<UserZugriffsrechte> UserZugriffsrechte { get; set; }
    }
}
