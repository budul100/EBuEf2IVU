using Microsoft.EntityFrameworkCore;
using DatabaseConnector.Models;

namespace DatabaseConnector.Contexts
{
    internal class FahrplanpunktContext(string connectionString)
        : BaseContext(connectionString)
    {
        #region Public Properties

        public DbSet<Fahrplanpunkt> Fahrplanpunkte { get; set; }

        #endregion Public Properties
    }
}