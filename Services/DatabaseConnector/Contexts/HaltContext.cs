using DatabaseConnector.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseConnector.Contexts
{
    internal class HaltContext
        : BaseContext
    {
        #region Public Constructors

        public HaltContext(string connectionString)
            : base(connectionString)
        { }

        #endregion Public Constructors

        #region Public Properties

        public DbSet<DispoHalt> Halte { get; set; }

        #endregion Public Properties
    }
}