using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseConnector.Models
{
    [Table("fahrplan_ffz")]
    internal class HaltPlan
        : Halt<ZugPlan>
    {
        #region Public Properties

        [ForeignKey(nameof(ZugID))]
        public override ZugPlan Zug { get; set; }

        #endregion Public Properties
    }
}