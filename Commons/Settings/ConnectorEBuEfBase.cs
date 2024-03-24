namespace Commons.Settings
{
    public abstract class ConnectorEBuEfBase
    {
        #region Public Properties

        public string MqttServer { get; set; }

        public string MqttTopic { get; set; }

        public string MulticastHost { get; set; }

        public int MulticastPort { get; set; }

        public int RetryTime { get; set; }

        public bool UseMulticast { get; set; }

        #endregion Public Properties
    }
}