using Microsoft.EntityFrameworkCore;
using EBuEf2IVU.Services.DatabaseConnector.Models;

namespace EBuEf2IVU.Services.DatabaseConnector.Contexts
{
    internal class SitzungContext(string connectionString)
        : BaseContext(connectionString)
    {
        #region Public Properties

        public DbSet<Sitzung> Sitzungen { get; set; }

        #endregion Public Properties
    }
}