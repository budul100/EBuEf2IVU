using Microsoft.EntityFrameworkCore;
using DatabaseConnector.Models;

namespace DatabaseConnector.Contexts
{
    internal class ZugGattungContext(string connectionString)
        : BaseContext(connectionString)
    {
        #region Public Properties

        public DbSet<Zuggattung> Zuggattungen { get; set; }

        #endregion Public Properties
    }
}