using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseConnector.Models
{
    internal abstract class Zug
    {
        #region Public Properties

        [Column("bemerkungen")]
        public string Bemerkungen { get; set; }

        [Column("verkehrstage_bin")]
        public string Bitmask { get; set; }

        [Key]
        [Column("id")]
        public int ID { get; set; }

        [ForeignKey(nameof(ZuggattungId))]
        public Zuggattung Zuggattung { get; set; }

        [Column("zuggattung_id")]
        public int ZuggattungId { get; set; }

        [Column("zugnummer")]
        public int Zugnummer { get; set; }

        #endregion Public Properties
    }
}