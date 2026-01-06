namespace EBuEf2IVU.Shareds.Commons.Settings
{
    public class CrewChecker
        : ConnectorIVUBase
    {
        #region Public Properties

        public int AbfrageIntervalSek { get; set; }

        public int AbfrageVergangenheitMin { get; set; }

        public int AbfrageZukunftMin { get; set; }

        public string Division { get; set; }

        public string PlanningLevel { get; set; }

        public int? RetryTime { get; set; }

        #endregion Public Properties
    }
}