using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class UserZugriffsrechte
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RechtId { get; set; }

        public virtual UserRechteliste Recht { get; set; }
        public virtual User User { get; set; }
    }
}
