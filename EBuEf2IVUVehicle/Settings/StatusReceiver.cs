namespace EBuEf2IVUVehicle.Settings
{
    internal class StatusReceiver
    {
        #region Public Properties

        public string IpAddress { get; set; }

        public int ListenerPort { get; set; }

        public string Pattern { get; set; }

        public int RetryTime { get; set; }

        public string StatusPattern { get; set; }

        #endregion Public Properties
    }
}