using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class ZuglenkungAktuell
    {
        public int Id { get; set; }
        public int Zugnummer { get; set; }
        public int ZugId { get; set; }
        public int GbtId { get; set; }
        public int FreimeldeabschnittId { get; set; }
        public int FahrstrasseId { get; set; }
        public TimeSpan? Abfahrtzeit { get; set; }
        public int Wartezeit { get; set; }
        public int EinstellungErfolgt { get; set; }
        public string Lu { get; set; }
        public int Dispohalt { get; set; }
        public int Reihenfolge { get; set; }
    }
}
