using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseConnector.Models
{
    [Table("fahrplan_sessionfahrplan")]
    internal class Halt
    {
        #region Public Properties

        [Column("abfahrt_ist")]
        public TimeSpan? AbfahrtIst { get; set; }

        [Column("ankunft_ist")]
        public TimeSpan? AnkunftIst { get; set; }

        [Column("betriebsstelle")]
        public string Betriebsstelle { get; set; }

        [Column("gleis_ist")]
        public string GleisIst { get; set; }

        [Key]
        [Column("id")]
        public int ID { get; set; }

        [ForeignKey(nameof(ZugID))]
        public Zug Zug { get; set; }

        [Column("zug_id")]
        public int ZugID { get; set; }

        #endregion Public Properties
    }
}