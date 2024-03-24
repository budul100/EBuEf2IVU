namespace Commons.Settings
{
    public class StatusReceiver
        : ConnectorEBuEfBase
    {
        #region Public Properties

        public string StartPattern { get; set; }

        public string StatusPattern { get; set; }

        #endregion Public Properties
    }
}