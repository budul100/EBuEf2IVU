using Microsoft.EntityFrameworkCore;
using DatabaseConnector.Models;

namespace DatabaseConnector.Contexts
{
    internal class BesatzungContext(string connectionString)
        : BaseContext(connectionString)
    {
        #region Public Properties

        public DbSet<Besatzung> Besatzungen { get; set; }

        #endregion Public Properties
    }
}