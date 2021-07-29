using Microsoft.EntityFrameworkCore;

namespace DatabaseConnector.Contexts
{
    internal abstract class BaseContext
        : DbContext
    {
        #region Private Fields

        private readonly string connectionString;

        #endregion Private Fields

        #region Public Constructors

        public BaseContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                connectionString: connectionString,
                serverVersion: ServerVersion.AutoDetect(connectionString));
        }

        #endregion Protected Methods
    }
}