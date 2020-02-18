using DatabaseConnector.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseConnector.Contexts
{
    internal class ZuegeContext
        : BaseContext
    {
        #region Public Constructors

        public ZuegeContext(string connectionString)
            : base(connectionString)
        { }

        #endregion Public Constructors

        #region Public Properties

        public DbSet<Zug> Zuege { get; set; }

        #endregion Public Properties
    }
}