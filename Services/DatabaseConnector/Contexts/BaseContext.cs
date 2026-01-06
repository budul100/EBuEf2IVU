using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace EBuEf2IVU.Services.DatabaseConnector.Contexts
{
    internal abstract class BaseContext(string connectionString)
        : DbContext
    {
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