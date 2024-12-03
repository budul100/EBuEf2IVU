namespace Commons.Settings
{
    public abstract class ConnectorIVUBase
    {
        #region Public Fields

        public const string EnvironmentHost = "IVU_APPSERVER_HOST";
        public const string EnvironmentIsHttps = "IVU_APPSERVER_ISHTTPS";
        public const string EnvironmentPort = "IVU_APPSERVER_PORT";

        public const int IVUAppServerPortDefault = 20000;

        #endregion Public Fields

        #region Public Properties

        public string Host { get; set; }

        public bool? IsHttps { get; set; }

        public string Password { get; set; }

        public string Path { get; set; }

        public int? Port { get; set; }

        public int? TimeoutInSecs { get; set; }

        public string Username { get; set; }

        #endregion Public Properties
    }
}