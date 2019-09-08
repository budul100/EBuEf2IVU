using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBuEfDBConnector.Models
{
    [Table("fma")]
    internal class Abschnitt
    {
        #region Public Properties

        [Column("decoder_adresse")]
        public string Decoder { get; set; }

        [Key]
        [Column("fma_id")]
        public int ID { get; set; }

        [Column("timestamp")]
        public DateTime Zeitpunkt { get; set; }

        #endregion Public Properties
    }
}