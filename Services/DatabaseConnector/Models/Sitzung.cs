using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseConnector.Models
{
    [Table("fahrplan_session")]
    internal class Sitzung
    {
        #region Public Properties

        [Column("id")]
        public int Id { get; set; }

        [Column("sim_ivutag")]
        public DateTime? IvuDate { get; set; }

        [Column("sessionkey")]
        public string SessionKey { get; set; }

        [Column("sim_startzeit")]
        public int SimulationStartzeit { get; set; }

        [Column("status")]
        public byte Status { get; set; }

        [Column("timeshift")]
        public int Verschiebung { get; set; }

        #endregion Public Properties
    }
}