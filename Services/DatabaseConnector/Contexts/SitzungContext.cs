using Microsoft.EntityFrameworkCore;
using DatabaseConnector.Models;

namespace DatabaseConnector.Contexts
{
    internal class SitzungContext(string connectionString)
        : BaseContext(connectionString)
    {
        #region Public Properties

        public DbSet<Sitzung> Sitzungen { get; set; }

        #endregion Public Properties
    }
}