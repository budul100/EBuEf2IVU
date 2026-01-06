using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBuEf2IVU.Services.DatabaseConnector.Models
{
    [Table("fahrplan_sessionfahrplan")]
    internal class HaltDispo
        : Halt<ZugDispo>
    {
        #region Public Properties

        [Column("abfahrt_ist")]
        public TimeSpan? AbfahrtIst { get; set; }

        [Column("abfahrt_prognose")]
        public TimeSpan? AbfahrtPrognose { get; set; }

        [Column("abfahrt_soll")]
        public TimeSpan? AbfahrtSoll { get; set; }

        [Column("ankunft_ist")]
        public TimeSpan? AnkunftIst { get; set; }

        [Column("ankunft_prognose")]
        public TimeSpan? AnkunftPrognose { get; set; }

        [Column("ankunft_soll")]
        public TimeSpan? AnkunftSoll { get; set; }

        [Column("gleis_ist")]
        public int? GleisIst { get; set; }

        [Column("gleis_soll")]
        public int? GleisSoll { get; set; }

        [ForeignKey(nameof(ZugID))]
        public override ZugDispo Zug { get; set; }

        #endregion Public Properties
    }
}