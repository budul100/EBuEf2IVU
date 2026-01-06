using Microsoft.EntityFrameworkCore;
using EBuEf2IVU.Services.DatabaseConnector.Models;

namespace EBuEf2IVU.Services.DatabaseConnector.Contexts
{
    internal class ZugDispoContext(string connectionString)
        : BaseContext(connectionString)
    {
        #region Public Properties

        public DbSet<ZugDispo> Zuege { get; set; }

        #endregion Public Properties
    }
}