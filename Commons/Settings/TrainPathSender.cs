namespace Common.Settings
{
    public class TrainPathSender
        : ConnectorIVUBase
    {
        #region Public Fields

        public const string SettingsSeparator = ",";

        #endregion Public Fields

        #region Public Properties

        public string IgnoreTrainTypes { get; set; }

        public string ImportProfile { get; set; }

        public string InfrastructureManager { get; set; }

        public string LocationTypes { get; set; }

        public bool LogRequests { get; set; }

        public string OrderingTransportationCompany { get; set; }

        public bool PreferPrognosis { get; set; }

        public int RetryTime { get; set; }

        public string StoppingReasonPass { get; set; }

        public string StoppingReasonStop { get; set; }

        public string TrainPathStateCancelled { get; set; }

        public string TrainPathStateRun { get; set; }

        #endregion Public Properties
    }
}