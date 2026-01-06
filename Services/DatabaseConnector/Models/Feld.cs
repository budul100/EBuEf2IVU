using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBuEf2IVU.Services.DatabaseConnector.Models
{
    [Table("zn_feld")]
    internal class Feld
    {
        #region Public Properties

        [ForeignKey(nameof(ID))]
        public IEnumerable<AbschnittZuFeld> AbschnittZuFeld { get; set; }

        [Column("fahrplanpunkt")]
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