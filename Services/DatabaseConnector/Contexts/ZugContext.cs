using DatabaseConnector.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseConnector.Contexts
{
    internal class ZugContext
        : BaseContext
    {
        #region Public Constructors

        public ZugContext(string connectionString)
            : base(connectionString)
        { }

        #endregion Public Constructors

        #region Public Properties

        public DbSet<Zug> Zuege { get; set; }

        #endregion Public Properties
    }
}