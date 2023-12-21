namespace Commons.Settings
{
    public abstract class ConnectorIVUBase
    {
        #region Public Properties

        public string Host { get; set; }

        public bool? IsHttps { get; set; }

        public string Password { get; set; }

        public string Path { get; set; }

        public int? Port { get; set; }

        public string Username { get; set; }

        #endregion Public Properties
    }
}