namespace Commons.Interfaces
{
    public interface IMQTTReceiver
        : IMessageReceiver
    {
        #region Public Methods

        void Initialize(string server, string topic, int retryTime, string messageType);

        #endregion Public Methods
    }
}