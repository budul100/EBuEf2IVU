using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class SignaleVorsignale
    {
        public int Id { get; set; }
        public int HauptsignalbegriffId { get; set; }
        public int? VorsignalbegriffId { get; set; }
        public int? HauptvorsignalbegriffId { get; set; }
        public sbyte? IsFahrstrassenabhaengig { get; set; }

        public virtual SignaleBegriffe Hauptsignalbegriff { get; set; }
        public virtual SignaleBegriffe Hauptvorsignalbegriff { get; set; }
        public virtual SignaleBegriffe Vorsignalbegriff { get; set; }
    }
}
