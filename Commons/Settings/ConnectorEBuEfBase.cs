namespace Commons.Settings
{
    public abstract class ConnectorEBuEfBase
    {
        #region Public Properties

        public string Host { get; set; }

        public int Port { get; set; }

        public int RetryTime { get; set; }

        #endregion Public Properties
    }
}