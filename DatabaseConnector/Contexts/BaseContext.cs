using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

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
            var conntectionStringBuilder = new MySqlConnectionStringBuilder(connectionString);
            conntectionStringBuilder.TreatTinyAsBoolean = false;

            optionsBuilder
                .UseMySql(conntectionStringBuilder.ToString());
        }

        #endregion Protected Methods
    }
}