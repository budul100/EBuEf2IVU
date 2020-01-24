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

        [Column("sim_startzeit")]
        public long SimulationStartzeit { get; set; }

        [Column("status")]
        public byte Status { get; set; }

        #endregion Public Properties
    }
}