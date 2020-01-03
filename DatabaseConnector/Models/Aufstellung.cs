using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseConnector.Models
{
    [Table("fahrplan_grundaufstellung")]
    internal class Aufstellung
    {
        #region Public Properties

        [Column("fahrzeug_adresse")]
        public string Decoder { get; set; }

        [ForeignKey(nameof(FeldID))]
        public Feld Feld { get; set; }

        [Key]
        [Column("gbt_id")]
        public int FeldID { get; set; }

        [Column("zugnummer")]
        public int Zugnummer { get; set; }

        #endregion Public Properties
    }
}