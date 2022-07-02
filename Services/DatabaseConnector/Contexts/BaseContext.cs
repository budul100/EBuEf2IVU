using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;

namespace DatabaseConnector.Contexts
{
    internal abstract class BaseContext
        : DbContext
    {
        #region Private Fields

        private readonly string connectionString;

        #endregion Private Fields

        #region Protected Constructors

        protected BaseContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        #endregion Protected Constructors

        #region Protected Methods

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                connectionString: connectionString,
                serverVersion: ServerVersion.AutoDetect(connectionString),
                mySqlOptionsAction: o => o.DefaultDataTypeMappings(NoneBooleanTypeMapping()));
        }

        #endregion Protected Methods

        #region Private Methods

        private static Func<MySqlDefaultDataTypeMappings, MySqlDefaultDataTypeMappings> NoneBooleanTypeMapping()
        {
            return m => m.WithClrBoolean(MySqlBooleanType.None);
        }

        #endregion Private Methods
    }
}