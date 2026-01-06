namespace EBuEf2IVU.Shareds.Commons.Settings
{
    public class EBuEfDBConnector
    {
        #region Public Fields

        public const string EnvironmentDBHost = "MYSQL_STD_HOST";
        public const string EnvironmentDBName = "MYSQL_STD_DBNAME";
        public const string EnvironmentDBPassword = "MYSQL_STD_PASSWORD";
        public const string EnvironmentDBUser = "MYSQL_STD_USER";

        #endregion Public Fields

        #region Public Properties

        public string ConnectionString { get; set; }

        public int? RetryTime { get; set; }

        #endregion Public Properties
    }
}