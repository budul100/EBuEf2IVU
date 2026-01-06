using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBuEf2IVU.Services.DatabaseConnector.Models
{
    [Table("fahrplan_grundaufstellung")]
    internal class Aufstellung
    {
        #region Public Properties

        [Column("fahrzeug_adresse")]
        public int Decoder { get; set; }

        [ForeignKey(nameof(FeldID))]
        public Feld Feld { get; set; }

        [Key]
        [Column("gbt_id")]
        public int FeldID { get; set; }

        [Column("zugnummer")]
        public int? Zugnummer { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return $"{Feld.Betriebsstelle} ({Feld.Gleis}): Zug {Zugnummer} / Fahrzeug {Decoder}";
        }

        #endregion Public Methods
    }
}