using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseConnector.Models
{
    [Table("fahrplan_sessioncrewing")]
    internal class Besatzung
    {
        #region Public Properties

        [Column("betriebsstelle_ende")]
        public string BetriebsstelleNach { get; set; }

        [Column("betriebsstelle_anfang")]
        public string BetriebsstelleVon { get; set; }

        [Column("dienst")]
        public string Dienst { get; set; }

        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("personalnachname")]
        public string PersonalNachname { get; set; }

        [Column("personalnummer")]
        public int PersonalNummer { get; set; }

        [Column("vorgaenger_zug_id")]
        public int? VorgaengerZugId { get; set; }

        [Column("zug_id")]
        public int ZugId { get; set; }

        #endregion Public Properties
    }
}