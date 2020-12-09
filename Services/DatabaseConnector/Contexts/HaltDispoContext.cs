using DatabaseConnector.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseConnector.Contexts
{
    internal class HaltDispoContext
        : BaseContext
    {
        #region Public Constructors

        public HaltDispoContext(string connectionString)
            : base(connectionString)
        { }

        #endregion Public Constructors

        #region Public Properties

        public DbSet<HaltDispo> Halte { get; set; }

        #endregion Public Properties
    }
}