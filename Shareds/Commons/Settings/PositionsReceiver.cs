namespace EBuEf2IVU.Shareds.Commons.Settings
{
    public class PositionsReceiver
        : ConnectorEBuEfBase
    {
        #region Public Fields

        public const string EnvironmentFormat = "MESSAGE_FORMAT_VEHICLE";

        public const string EnvironmentMCHost = "MC_HOST_VEHICLE";
        public const string EnvironmentMCPort = "MC_PORT_VEHICLE";

        public const string EnvironmentMQTTHost = "MQTT_BROKER_VEHICLE";
        public const string EnvironmentMQTTPort = "MQTT_PORT_VEHICLE";
        public const string EnvironmentMQTTTopic = "MQTT_TOPIC_VEHICLE";

        public const int MulticastPortDefault = 4455;

        #endregion Public Fields
    }
}