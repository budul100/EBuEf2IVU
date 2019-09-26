using Serilog.Events;

namespace Common.Settings
{
    public class EBuEf2IVUSettings
    {
        #region Public Properties

        public string AllocationsIpAddress { get; set; }

        public int AllocationsListenerPort { get; set; }

        public string AllocationsPattern { get; set; }

        public int AllocationsRetryTime { get; set; }

        public string DatabaseConnectionString { get; set; }

        public int DatabaseRetryTime { get; set; }

        public InfrastructureMapping[] InfrastructureMappings { get; set; }

        public string LogFilePath { get; set; }

        public LogEventLevel? LogLevel { get; set; } = LogEventLevel.Information;

        public string MessagesIpAddress { get; set; }

        public int MessagesListenerPort { get; set; }

        public int MessagesRetryTime { get; set; }

        public string PerformanceTestBetriebsstelleVon { get; set; }

        public string PerformanceTestZugnummer { get; set; }

        public string SenderDivision { get; set; }

        public string SenderEndpoint { get; set; }

        public int SenderRetryTime { get; set; }

        #endregion Public Properties
    }
}