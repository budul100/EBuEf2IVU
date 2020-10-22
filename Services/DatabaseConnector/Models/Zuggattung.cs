using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseConnector.Models
{
    [Table("zuege_zuggattungen")]
    internal class Zuggattung
    {
        #region Public Properties

        [Column("fzs")]
        public string FzsFahrtart { get; set; }

        [Key]
        [Column("id")]
        public int ID { get; set; }

        [Column("zuggattung")]
        public string Kurzname { get; set; }

        [Column("bezeichnung")]
        public string Langname { get; set; }

        [Column("verkehrsart")]
        public string Verkehrsart { get; set; }

        #endregion Public Properties
    }
}