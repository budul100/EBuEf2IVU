namespace EBuEf2IVUBase.Settings
{
    public class StatusReceiver
    {
        #region Public Properties

        public string IpAddress { get; set; }

        public int ListenerPort { get; set; }

        public int RetryTime { get; set; }

        public string StartPattern { get; set; }

        public string StatusPattern { get; set; }

        #endregion Public Properties
    }
}