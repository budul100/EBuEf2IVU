using Microsoft.EntityFrameworkCore;
using DatabaseConnector.Models;

namespace DatabaseConnector.Contexts
{
    internal class ZugDispoContext(string connectionString)
        : BaseContext(connectionString)
    {
        #region Public Properties

        public DbSet<ZugDispo> Zuege { get; set; }

        #endregion Public Properties
    }
}