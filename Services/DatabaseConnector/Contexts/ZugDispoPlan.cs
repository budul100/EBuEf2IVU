using DatabaseConnector.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseConnector.Contexts
{
    internal class ZugDispoPlan
        : BaseContext
    {
        #region Public Constructors

        public ZugDispoPlan(string connectionString)
            : base(connectionString)
        { }

        #endregion Public Constructors

        #region Public Properties

        public DbSet<ZugDispo> Zuege { get; set; }

        #endregion Public Properties
    }
}