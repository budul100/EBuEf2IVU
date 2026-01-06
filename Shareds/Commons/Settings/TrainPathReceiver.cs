namespace EBuEf2IVU.Shareds.Commons.Settings
{
    public class TrainPathReceiver
        : ConnectorEBuEfBase
    {
        #region Public Fields

        public const string EnvironmentFormat = "MESSAGE_FORMAT_PATH";

        public const string EnvironmentMCHost = "MC_HOST_PATH";
        public const string EnvironmentMCPort = "MC_PORT_PATH";

        public const string EnvironmentMQTTHost = "MQTT_BROKER_PATH";
        public const string EnvironmentMQTTPort = "MQTT_PORT_PATH";
        public const string EnvironmentMQTTTopic = "MQTT_TOPIC_PATH";

        public const int MulticastPortDefault = 4455;

        #endregion Public Fields
    }
}