using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseConnector.Models
{
    [Table("gbt_fma")]
    internal class AbschnittZuFeld
    {
        #region Public Properties

        [ForeignKey(nameof(AbschnittID))]
        public Abschnitt Abschnitt { get; set; }

        [Column("fma_id")]
        public int? AbschnittID { get; set; }

        [ForeignKey(nameof(FeldID))]
        public Feld Feld { get; set; }

        [Key]
        [Column("gbt_id")]
        public int? FeldID { get; set; }

        #endregion Public Properties
    }
}