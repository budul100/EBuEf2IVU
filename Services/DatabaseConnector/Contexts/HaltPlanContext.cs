using Microsoft.EntityFrameworkCore;
using DatabaseConnector.Models;

namespace DatabaseConnector.Contexts
{
    internal class HaltPlanContext(string connectionString)
        : BaseContext(connectionString)
    {
        #region Public Properties

        public DbSet<HaltPlan> Halte { get; set; }

        #endregion Public Properties
    }
}