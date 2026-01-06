using Microsoft.EntityFrameworkCore;
using EBuEf2IVU.Services.DatabaseConnector.Models;

namespace EBuEf2IVU.Services.DatabaseConnector.Contexts
{
    internal class BesatzungContext(string connectionString)
        : BaseContext(connectionString)
    {
        #region Public Properties

        public DbSet<Besatzung> Besatzungen { get; set; }

        #endregion Public Properties
    }
}