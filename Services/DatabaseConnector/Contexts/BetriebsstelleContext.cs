using DatabaseConnector.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseConnector.Contexts
{
    internal class BetriebsstelleContext
        : BaseContext
    {
        #region Public Constructors

        public BetriebsstelleContext(string connectionString)
            : base(connectionString)
        { }

        #endregion Public Constructors

        #region Public Properties

        public DbSet<Betriebsstelle> Betriebsstellen { get; set; }

        #endregion Public Properties
    }
}