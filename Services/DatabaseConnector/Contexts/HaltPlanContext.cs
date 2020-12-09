using DatabaseConnector.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseConnector.Contexts
{
    internal class HaltPlanContext
        : BaseContext
    {
        #region Public Constructors

        public HaltPlanContext(string connectionString)
            : base(connectionString)
        { }

        #endregion Public Constructors

        #region Public Properties

        public DbSet<HaltPlan> Halte { get; set; }

        #endregion Public Properties
    }
}