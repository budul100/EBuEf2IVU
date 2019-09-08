using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class ErlaubnisfelderAufhebung
    {
        public int Id { get; set; }
        public int ErlaubnisfeldId { get; set; }
        public int UserAktivId { get; set; }

        public virtual UserAnmeldungenAktiv UserAktiv { get; set; }
    }
}
