namespace EBuEf2IVU.Shareds.Commons.Interfaces
{
    public interface IMulticastReceiver
        : IMessageReceiver
    {
        #region Public Methods

        void Initialize(string host, int port, int? retryTime, string messageType);

        #endregion Public Methods
    }
}