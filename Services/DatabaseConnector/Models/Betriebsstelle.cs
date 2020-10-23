using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseConnector.Models
{
    [Table("betriebsstellen_liste")]
    internal class Betriebsstelle
    {
        #region Public Properties

        [Column("art")]
        public string Art { get; set; }

        [Column("bstart")]
        public string BetriebsstellenArt { get; set; }

        [Key]
        [Column("id")]
        public int ID { get; set; }

        [Column("kuerzel")]
        public string Kurzname { get; set; }

        [Column("name")]
        public string Langname { get; set; }

        [Column("nummer")]
        public int Nummer { get; set; }

        #endregion Public Properties
    }
}