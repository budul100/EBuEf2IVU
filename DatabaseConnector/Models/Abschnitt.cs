using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseConnector.Models
{
    [Table("fma")]
    internal class Abschnitt
    {
        #region Public Properties

        public AbschnittZuFeld AbschnittZuFeld { get; set; }

        [Column("decoder_adresse")]
        public int Decoder { get; set; }

        [Key]
        [Column("fma_id")]
        public int ID { get; set; }

        [Column("timestamp")]
        public DateTime Zeitpunkt { get; set; }

        #endregion Public Properties
    }
}