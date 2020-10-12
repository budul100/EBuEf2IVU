namespace EBuEf2IVUCrew.Settings
{
    public class CrewChecker
    {
        #region Public Properties

        public int AbfrageIntervalSek { get; set; }

        public int AbfrageVergangenheitMin { get; set; }

        public int AbfrageZukunftMin { get; set; }

        public string Division { get; set; }

        public string Host { get; set; }

        public bool IsHttps { get; set; }

        public string Password { get; set; }

        public string Path { get; set; }

        public string PlanningLevel { get; set; }

        public int Port { get; set; }

        public int RetryTime { get; set; }

        public string Username { get; set; }

        #endregion Public Properties
    }
}