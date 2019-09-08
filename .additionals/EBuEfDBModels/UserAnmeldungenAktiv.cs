using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class UserAnmeldungenAktiv
    {
        public UserAnmeldungenAktiv()
        {
            ErlaubnisfelderAufhebung = new HashSet<ErlaubnisfelderAufhebung>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public sbyte IsTrainer { get; set; }
        public string Host { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual ICollection<ErlaubnisfelderAufhebung> ErlaubnisfelderAufhebung { get; set; }
    }
}
