using Microsoft.EntityFrameworkCore;

namespace EBuEfDBConnector.Models
{
    internal class AufstellungenContext : BaseContext
    {
        #region Public Constructors

        public AufstellungenContext(string connectionString)
            : base(connectionString)
        { }

        #endregion Public Constructors

        #region Public Properties

        public DbSet<Aufstellung> Aufstellungen { get; set; }

        #endregion Public Properties
    }
}