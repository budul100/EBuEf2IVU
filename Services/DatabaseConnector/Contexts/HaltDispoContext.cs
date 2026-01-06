using Microsoft.EntityFrameworkCore;
using EBuEf2IVU.Services.DatabaseConnector.Models;

namespace EBuEf2IVU.Services.DatabaseConnector.Contexts
{
    internal class HaltDispoContext(string connectionString)
        : BaseContext(connectionString)
    {
        #region Public Properties

        public DbSet<HaltDispo> Halte { get; set; }

        #endregion Public Properties
    }
}