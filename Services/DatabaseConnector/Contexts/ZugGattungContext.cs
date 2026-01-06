using Microsoft.EntityFrameworkCore;
using EBuEf2IVU.Services.DatabaseConnector.Models;

namespace EBuEf2IVU.Services.DatabaseConnector.Contexts
{
    internal class ZugGattungContext(string connectionString)
        : BaseContext(connectionString)
    {
        #region Public Properties

        public DbSet<ZugGattung> Zuggattungen { get; set; }

        #endregion Public Properties
    }
}