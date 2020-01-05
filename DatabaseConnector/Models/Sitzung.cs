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

        #endregion Public Properties
    }
}