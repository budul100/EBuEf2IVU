using DatabaseConnector.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseConnector.Contexts
{
    internal class ZugGattungContext
        : BaseContext
    {
        #region Public Constructors

        public ZugGattungContext(string connectionString)
            : base(connectionString)
        { }

        #endregion Public Constructors

        #region Public Properties

        public DbSet<Zuggattung> Zuggattungen { get; set; }

        #endregion Public Properties
    }
}