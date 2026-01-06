namespace EBuEf2IVU.Shareds.Commons.Settings
{
    public class StatusReceiver
        : ConnectorEBuEfBase
    {
        #region Public Fields

        public const string EnvironmentFormat = "MESSAGE_FORMAT_STATUS";

        public const string EnvironmentMCHost = "MC_HOST_STATUS";
        public const string EnvironmentMCPort = "MC_PORT_STATUS";

        public const string EnvironmentMQTTHost = "MQTT_BROKER_STATUS";
        public const string EnvironmentMQTTPort = "MQTT_PORT_STATUS";
        public const string EnvironmentMQTTTopic = "MQTT_TOPIC_STATUS";

        public const int MulticastPortDefault = 4454;

        #endregion Public Fields

        #region Public Properties

        public string StartPattern { get; set; }

        public string StatusPattern { get; set; }

        #endregion Public Properties
    }
}