using DatabaseConnector.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseConnector.Contexts
{
    internal class FahrplanpunktContext
        : BaseContext
    {
        #region Public Constructors

        public FahrplanpunktContext(string connectionString)
            : base(connectionString)
        { }

        #endregion Public Constructors

        #region Public Properties

        public DbSet<Fahrplanpunkt> Fahrplanpunkte { get; set; }

        #endregion Public Properties
    }
}