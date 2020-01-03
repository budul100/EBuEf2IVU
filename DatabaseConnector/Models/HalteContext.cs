using Microsoft.EntityFrameworkCore;

namespace DatabaseConnector.Models
{
    internal class HalteContext : BaseContext
    {
        #region Public Constructors

        public HalteContext(string connectionString)
            : base(connectionString)
        { }

        #endregion Public Constructors

        #region Public Properties

        public DbSet<Halt> Halte { get; set; }

        #endregion Public Properties
    }
}