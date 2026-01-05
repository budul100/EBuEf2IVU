using Microsoft.EntityFrameworkCore;
using DatabaseConnector.Models;

namespace DatabaseConnector.Contexts
{
    internal class HaltDispoContext(string connectionString)
        : BaseContext(connectionString)
    {
        #region Public Properties

        public DbSet<HaltDispo> Halte { get; set; }

        #endregion Public Properties
    }
}