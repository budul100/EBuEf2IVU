using Common.Enums;
using Common.EventsArgs;
using Common.Extensions;
using Common.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace StateHandler
{
    public class Handler
        : IStateHandler
    {
        #region Private Fields

        private const string MessageTypeState = "Sessionstatus";
        private const string StatusRegexGroupName = "status";
        private const string StatusRegexGroupWildcard = "$";

        private readonly IDatabaseConnector databaseConnector;
        private readonly ILogger logger;
        private readonly IMessageReceiver stateReceiver;

        private Regex sessionStartRegex;
        private Regex sessionStatusRegex;

        #endregion Private Fields

        #region Public Constructors

        public Handler(ILogger<Handler> logger, IDatabaseConnector databaseConnector, IMessageReceiver stateReceiver)
        {
            this.logger = logger;

            this.databaseConnector = databaseConnector;
            this.stateReceiver = stateReceiver;
            this.stateReceiver.MessageReceivedEvent += OnStatusReceived;
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler<StateChangedArgs> SessionChangedEvent;

        public event EventHandler SessionStartedEvent;

        #endregion Public Events

        #region Public Methods

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var isSessionStarted = await IsSessionStartedAsync();

            if (isSessionStarted)
            {
                SendSessionStatus(SessionStatusType.IsRunning);
            }

            await stateReceiver.ExecuteAsync(cancellationToken);
        }

        public void Initialize(string host, int port, int retryTime, string sessionStartPattern,
            string sessionStatusPattern)
        {
            stateReceiver.Initialize(
                host: host,
                port: port,
                retryTime: retryTime,
                messageType: MessageTypeState);

            sessionStartRegex = GetSessionStartRegex(sessionStartPattern);
            sessionStatusRegex = GetSessionStatusRegex(sessionStatusPattern);
        }

        #endregion Public Methods

        #region Private Methods

        private static Regex GetSessionStartRegex(string startPattern)
        {
            return new Regex(startPattern);
        }

        private Regex GetSessionStatusRegex(string statusPattern)
        {
            // The new value cannot be created with \d!

            var correctedPattern = statusPattern.Replace(
                oldValue: StatusRegexGroupWildcard,
                newValue: @$"(?<{StatusRegexGroupName}>[0-9])");

            var result = new Regex(correctedPattern);

            return result;
        }

        private async Task<bool> IsSessionStartedAsync()
        {
            var result = await databaseConnector.GetEBuEfSessionActiveAsync();

            return result;
        }

        private void OnStatusReceived(object sender, MessageReceivedArgs e)
        {
            logger.LogDebug(
                "Status-Nachricht empfangen: {content}",
                e.Content);

            if (sessionStartRegex.IsMatch(e.Content))
            {
                SessionStartedEvent?.Invoke(
                    sender: this,
                    e: null);
            }
            else
            {
                var sessionStatusText = sessionStatusRegex.IsMatch(e.Content)
                    ? sessionStatusRegex.Match(e.Content).Groups[StatusRegexGroupName].Value
                    : default;

                var sessionStatus = sessionStatusText.GetSessionStatusType();

                if (sessionStatus == default)
                {
                    logger.LogError(
                        "Unbekannte Status-Nachricht empfangen: {content}",
                        e.Content);
                }
                else
                {
                    SendSessionStatus(sessionStatus.Value);
                }
            }
        }

        private void SendSessionStatus(SessionStatusType sessionStatus)
        {
            switch (sessionStatus)
            {
                case SessionStatusType.InPreparation:
                    SessionChangedEvent?.Invoke(
                        sender: this,
                        e: new StateChangedArgs(SessionStatusType.InPreparation));
                    break;

                case SessionStatusType.IsRunning:
                    SessionChangedEvent?.Invoke(
                        sender: this,
                        e: new StateChangedArgs(SessionStatusType.IsRunning));
                    break;

                case SessionStatusType.IsEnded:
                    SessionChangedEvent?.Invoke(
                        sender: this,
                        e: new StateChangedArgs(SessionStatusType.IsEnded));
                    break;

                case SessionStatusType.IsPaused:
                    SessionChangedEvent?.Invoke(
                        sender: this,
                        e: new StateChangedArgs(SessionStatusType.IsPaused));
                    break;
            }
        }

        #endregion Private Methods
    }
}