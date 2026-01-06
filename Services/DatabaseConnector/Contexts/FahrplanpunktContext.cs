using Microsoft.EntityFrameworkCore;
using EBuEf2IVU.Services.DatabaseConnector.Models;

namespace EBuEf2IVU.Services.DatabaseConnector.Contexts
{
    internal class FahrplanpunktContext(string connectionString)
        : BaseContext(connectionString)
    {
        #region Public Properties

        public DbSet<FahrplanPunkt> Fahrplanpunkte { get; set; }

        #endregion Public Properties
    }
}