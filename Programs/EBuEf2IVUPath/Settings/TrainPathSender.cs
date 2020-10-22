namespace EBuEf2IVUPath.Settings
{
    public class TrainPathSender
    {
        #region Public Properties

        public string Host { get; set; }

        public string IgnoreTrainTypes { get; set; }

        public string ImportProfile { get; set; }

        public string InfrastructureManager { get; set; }

        public bool IsHttps { get; set; }

        public string OrderingTransportationCompany { get; set; }

        public string Password { get; set; }

        public string Path { get; set; }

        public int Port { get; set; }

        public bool PreferPrognosis { get; set; }

        public int RetryTime { get; set; }

        public string StoppingReasonPass { get; set; }

        public string StoppingReasonStop { get; set; }

        public string TrainPathState { get; set; }

        public string Username { get; set; }

        #endregion Public Properties
    }
}