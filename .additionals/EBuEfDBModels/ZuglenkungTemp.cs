using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class ZuglenkungTemp
    {
        public int Id { get; set; }
        public int LenkplanId { get; set; }
        public int InfraId { get; set; }
        public int BelegungTimestamp { get; set; }
        public int Zugnummer { get; set; }
        public int? ZugId { get; set; }
        public int? FahrstrasseId { get; set; }
        public int AnstosszeitTimestamp { get; set; }
        public int EinstellungZaehler { get; set; }
        public int EinstellungTimestamp { get; set; }
        public int Dispohalt { get; set; }
        public int Reihenfolge { get; set; }
    }
}
