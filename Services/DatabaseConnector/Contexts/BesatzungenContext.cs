using DatabaseConnector.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseConnector.Contexts
{
    internal class BesatzungenContext
        : BaseContext
    {
        #region Public Constructors

        public BesatzungenContext(string connectionString)
            : base(connectionString)
        { }

        #endregion Public Constructors

        #region Public Properties

        public DbSet<Besatzung> Besatzungen { get; set; }

        #endregion Public Properties
    }
}