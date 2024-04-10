namespace Commons.Interfaces
{
    public interface IMQTTReceiver
        : IMessageReceiver
    {
        #region Public Methods

        void Initialize(string server, int? port, string topic, int retryTime, string messageType);

        #endregion Public Methods
    }
}