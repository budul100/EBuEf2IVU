using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseConnector.Models
{
    [Table("fahrplan_sessionfahrplan")]
    internal class DispoHalt
    {
        #region Public Properties

        [Column("abfahrt_ist")]
        public TimeSpan? AbfahrtIst { get; set; }

        [Column("abfahrt_plan")]
        public TimeSpan? AbfahrtPlan { get; set; }

        [Column("abfahrt_prognose")]
        public TimeSpan? AbfahrtPrognose { get; set; }

        [Column("abfahrt_soll")]
        public TimeSpan? AbfahrtSoll { get; set; }

        [Column("ankunft_ist")]
        public TimeSpan? AnkunftIst { get; set; }

        [Column("ankunft_plan")]
        public TimeSpan? AnkunftPlan { get; set; }

        [Column("ankunft_prognose")]
        public TimeSpan? AnkunftPrognose { get; set; }

        [Column("ankunft_soll")]
        public TimeSpan? AnkunftSoll { get; set; }

        [Column("bemerkungen")]
        public string Bemerkungen { get; set; }

        [Column("betriebsstelle")]
        public string Betriebsstelle { get; set; }

        [Column("gleis_ist")]
        public int? GleisIst { get; set; }

        [Column("gleis_plan")]
        public int? GleisPlan { get; set; }

        [Column("gleis_soll")]
        public int? GleisSoll { get; set; }

        [Key]
        [Column("id")]
        public int ID { get; set; }

        [Column("ist_Durchfahrt")]
        public bool IstDurchfahrt { get; set; }

        [Column("sortierzeit")]
        public TimeSpan SortierZeit { get; set; }

        [ForeignKey(nameof(ZugID))]
        public DispoZug Zug { get; set; }

        [Column("zug_id")]
        public int ZugID { get; set; }

        #endregion Public Properties
    }
}