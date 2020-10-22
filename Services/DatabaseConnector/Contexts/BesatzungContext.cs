using DatabaseConnector.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseConnector.Contexts
{
    internal class BesatzungContext
        : BaseContext
    {
        #region Public Constructors

        public BesatzungContext(string connectionString)
            : base(connectionString)
        { }

        #endregion Public Constructors

        #region Public Properties

        public DbSet<Besatzung> Besatzungen { get; set; }

        #endregion Public Properties
    }
}