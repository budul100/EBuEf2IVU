using System;

namespace Commons.Settings
{
    public abstract class ConnectorEBuEfBase
    {
        #region Public Fields

        public const string FormatMulticast = "multicast";

        #endregion Public Fields

        #region Public Properties

        public string Host { get; set; }

        public int? Port { get; set; }

        public int RetryTime { get; set; }

        [Obsolete("This attribute has been replaced by the attribute " + nameof(Host) + ".")]
        public string Server
        {
            get { return Host; }
            set { Host = value; }
        }

        public string Topic { get; set; }

        public bool UseMulticast { get; set; }

        #endregion Public Properties
    }
}