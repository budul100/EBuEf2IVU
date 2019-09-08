using Microsoft.EntityFrameworkCore;

namespace EBuEfDBConnector.Models
{
    internal abstract class BaseContext : DbContext
    {
        #region Protected Fields

        protected readonly string connectionString;

        #endregion Protected Fields

        #region Public Constructors

        public BaseContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseMySql(connectionString);
        }

        #endregion Protected Methods
    }
}