using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class SignaleBegriffe
    {
        public SignaleBegriffe()
        {
            Fahrstrassen = new HashSet<Fahrstrassen>();
            FahrstrassenDwege = new HashSet<FahrstrassenDwege>();
            FahrstrassenElemente = new HashSet<FahrstrassenElemente>();
            FahrstrassenFolgenStartsignalbegriff = new HashSet<FahrstrassenFolgen>();
            FahrstrassenFolgenZielsignalbegriff = new HashSet<FahrstrassenFolgen>();
            InverseOriginalBegriff = new HashSet<SignaleBegriffe>();
            Signale = new HashSet<Signale>();
            SignaleElemente = new HashSet<SignaleElemente>();
            SignaleVorsignaleHauptsignalbegriff = new HashSet<SignaleVorsignale>();
            SignaleVorsignaleHauptvorsignalbegriff = new HashSet<SignaleVorsignale>();
            SignaleVorsignaleVorsignalbegriff = new HashSet<SignaleVorsignale>();
        }

        public int Id { get; set; }
        public int SignalId { get; set; }
        public int? Adresse { get; set; }
        public string Begriff { get; set; }
        public int IsZugfahrtbegriff { get; set; }
        public int Geschwindigkeit { get; set; }
        public string Zielgeschwindigkeit { get; set; }
        public string WebstwFarbe { get; set; }
        public int Zielentfernung { get; set; }
        public int? OriginalBegriffId { get; set; }

        public virtual SignaleBegriffe OriginalBegriff { get; set; }
        public virtual Signale Signal { get; set; }
        public virtual ICollection<Fahrstrassen> Fahrstrassen { get; set; }
        public virtual ICollection<FahrstrassenDwege> FahrstrassenDwege { get; set; }
        public virtual ICollection<FahrstrassenElemente> FahrstrassenElemente { get; set; }
        public virtual ICollection<FahrstrassenFolgen> FahrstrassenFolgenStartsignalbegriff { get; set; }
        public virtual ICollection<FahrstrassenFolgen> FahrstrassenFolgenZielsignalbegriff { get; set; }
        public virtual ICollection<SignaleBegriffe> InverseOriginalBegriff { get; set; }
        public virtual ICollection<Signale> Signale { get; set; }
        public virtual ICollection<SignaleElemente> SignaleElemente { get; set; }
        public virtual ICollection<SignaleVorsignale> SignaleVorsignaleHauptsignalbegriff { get; set; }
        public virtual ICollection<SignaleVorsignale> SignaleVorsignaleHauptvorsignalbegriff { get; set; }
        public virtual ICollection<SignaleVorsignale> SignaleVorsignaleVorsignalbegriff { get; set; }
    }
}
