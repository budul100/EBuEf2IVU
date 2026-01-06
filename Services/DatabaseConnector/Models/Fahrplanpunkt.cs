using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBuEf2IVU.Services.DatabaseConnector.Models
{
    [Table("fahrplanpunkt")]
    internal class FahrplanPunkt
    {
        #region Public Properties

        [Column("art")]
        public string Art { get; set; }

        [Key]
        [Column("id")]
        public int ID { get; set; }

        [Column("kuerzel")]
        public string Kurzname { get; set; }

        [Column("name")]
        public string Langname { get; set; }

        #endregion Public Properties
    }
}