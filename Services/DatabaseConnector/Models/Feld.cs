using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseConnector.Models
{
    [Table("gbt")]
    internal class Feld
    {
        #region Public Properties

        [ForeignKey(nameof(ID))]
        public IEnumerable<AbschnittZuFeld> AbschnittZuFeld { get; set; }

        [Column("betriebsstelle")]
        public string Betriebsstelle { get; set; }

        [Column("gleis")]
        public int? Gleis { get; set; }

        [Key]
        [Column("id")]
        public int ID { get; set; }

        [Column("kurzbezeichnung")]
        public string Name { get; set; }

        #endregion Public Properties
    }
}