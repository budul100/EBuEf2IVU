namespace EBuEf2IVUBase.Settings
{
    public class StatusReceiver
    {
        #region Public Properties

        public string Host { get; set; }

        public int Port { get; set; }

        public int RetryTime { get; set; }

        public string StartPattern { get; set; }

        public string StatusPattern { get; set; }

        #endregion Public Properties
    }
}