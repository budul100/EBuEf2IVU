using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class FahrzeugeAbschnitte
    {
        public int Id { get; set; }
        public int FahrzeugId { get; set; }
        public int AbschnittId { get; set; }
    }
}
