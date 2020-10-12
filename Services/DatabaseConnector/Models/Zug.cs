using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseConnector.Models
{
    [Table("fahrplan_sessionzuege")]
    internal class Zug
    {
        #region Public Properties

        [Column("bemerkungen")]
        public string Bemerkungen { get; set; }

        [Key]
        [Column("id")]
        public int ID { get; set; }

        [Column("zugnummer")]
        public int Zugnummer { get; set; }

        #endregion Public Properties
    }
}