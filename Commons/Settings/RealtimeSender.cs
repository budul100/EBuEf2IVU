using System;

namespace Common.Settings
{
    public class RealtimeSender
    {
        #region Public Properties

        public DateTime? DateMin { get; set; }

        public string Division { get; set; }

        public string Endpoint { get; set; }

        public string Host { get; set; }

        public bool IgnorePrognosis { get; set; }

        public bool IsHttps { get; set; }

        public string Password { get; set; }

        public string Path { get; set; }

        public int Port { get; set; }

        public int RetryTime { get; set; }

        public bool UseInterfaceServer { get; set; }

        public string Username { get; set; }

        #endregion Public Properties
    }
}