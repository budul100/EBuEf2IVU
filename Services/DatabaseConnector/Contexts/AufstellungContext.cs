using Microsoft.EntityFrameworkCore;
using EBuEf2IVU.Services.DatabaseConnector.Models;

namespace EBuEf2IVU.Services.DatabaseConnector.Contexts
{
    internal class AufstellungContext(string connectionString)
        : BaseContext(connectionString)
    {
        #region Public Properties

        public DbSet<Aufstellung> Aufstellungen { get; set; }

        #endregion Public Properties
    }
}