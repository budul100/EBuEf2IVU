using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class Betra
    {
        public int Id { get; set; }
        public string Nummer { get; set; }
        public DateTime GueltigAb { get; set; }
        public DateTime GueltigBis { get; set; }
        public string LageStrecke { get; set; }
        public string LageBahnhof { get; set; }
        public string LagePlan { get; set; }
        public string Arbeitszeit { get; set; }
        public string Sperrung { get; set; }
        public string SperrungHinweis { get; set; }
        public string Grund { get; set; }
        public string Evu { get; set; }
        public string Weitere { get; set; }
        public string PvAusfall { get; set; }
        public string PvNeu { get; set; }
        public string PvUmleitung { get; set; }
        public string PvBedarfsperre { get; set; }
        public string PvUebrige { get; set; }
        public string GvAusfall { get; set; }
        public string GvNeu { get; set; }
        public string GvUmleitung { get; set; }
        public string GvBedarfsperre { get; set; }
        public string GvUebrige { get; set; }
        public string Sonstiges { get; set; }
        public string Verfasser { get; set; }
    }
}
