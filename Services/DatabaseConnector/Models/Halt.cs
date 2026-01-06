using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBuEf2IVU.Services.DatabaseConnector.Models
{
    internal abstract class Halt<T>
        where T : Zug
    {
        #region Public Properties

        [Column("abfahrt_plan")]
        public TimeSpan? AbfahrtPlan { get; set; }

        [Column("ankunft_plan")]
        public TimeSpan? AnkunftPlan { get; set; }

        [Column("bemerkungen")]
        public string Bemerkungen { get; set; }

        [Column("betriebsstelle")]
        public string Betriebsstelle { get; set; }

        [Column("gleis_plan")]
        public int? GleisPlan { get; set; }

        [Key]
        [Column("id")]
        public int ID { get; set; }

        [Column("ist_Durchfahrt")]
        public bool IstDurchfahrt { get; set; }

        [Column("sortierzeit")]
        public TimeSpan SortierZeit { get; set; }

        public abstract T Zug { get; set; }

        [Column("zug_id")]
        public int ZugID { get; set; }

        #endregion Public Properties
    }
}