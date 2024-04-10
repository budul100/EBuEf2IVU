namespace Commons.Settings
{
    public abstract class ConnectorEBuEfBase
    {
        #region Public Fields

        public const int MulticastPort = 5355;

        #endregion Public Fields

        #region Public Properties

        public int? Port { get; set; } = MulticastPort;

        public int RetryTime { get; set; }

        public string Server { get; set; }

        public string Topic { get; set; }

        public bool UseMulticast { get; set; }

        #endregion Public Properties
    }
}