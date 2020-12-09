using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseConnector.Models
{
    [Table("fahrplan_zuege")]
    internal class ZugPlan
        : Zug
    {
        #region Public Properties

        [Column("fahrplanversion_id")]
        public int FahrplanId { get; set; }

        #endregion Public Properties
    }
}