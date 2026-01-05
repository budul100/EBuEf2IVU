using Microsoft.EntityFrameworkCore;
using DatabaseConnector.Models;

namespace DatabaseConnector.Contexts
{
    internal class AufstellungContext(string connectionString)
        : BaseContext(connectionString)
    {
        #region Public Properties

        public DbSet<Aufstellung> Aufstellungen { get; set; }

        #endregion Public Properties
    }
}