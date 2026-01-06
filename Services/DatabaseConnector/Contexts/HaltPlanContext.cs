using Microsoft.EntityFrameworkCore;
using EBuEf2IVU.Services.DatabaseConnector.Models;

namespace EBuEf2IVU.Services.DatabaseConnector.Contexts
{
    internal class HaltPlanContext(string connectionString)
        : BaseContext(connectionString)
    {
        #region Public Properties

        public DbSet<HaltPlan> Halte { get; set; }

        #endregion Public Properties
    }
}