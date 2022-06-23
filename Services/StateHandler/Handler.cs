using Common.Enums;
using Common.EventsArgs;
using Common.Extensions;
using Common.Interfaces;
using Microsoft.Extensions.Logging;
using StateHandler.Extensions;
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

        #endregion Public Events

        #region Public Properties

        public SessionStatusType SessionStatus { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await SetSessionStatusInitally();

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

            sessionStartRegex = sessionStartPattern.GetSessionStartRegex();
            sessionStatusRegex = sessionStatusPattern.GetSessionStatusRegex();
        }

        #endregion Public Methods

        #region Private Methods

        private SessionStatusType? GetSessionStatus(string message)
        {
            var sessionStatusText = sessionStatusRegex.IsMatch(message)
                ? sessionStatusRegex.Match(message).Groups[PatternExtensions.StatusRegexGroupName].Value
                : default;

            var sessionStatus = sessionStatusText.GetSessionStatusType();

            return sessionStatus;
        }

        private void OnStatusReceived(object sender, MessageReceivedArgs e)
        {
            logger.LogDebug(
                "Status-Nachricht empfangen: {content}",
                e.Content);

            if (sessionStartRegex.IsMatch(e.Content))
            {
                SetSessionStatus(SessionStatusType.IsRunning);
            }
            else
            {
                var sessionStatus = GetSessionStatus(e.Content);

                if (sessionStatus == default)
                {
                    logger.LogError(
                        "Unbekannte Status-Nachricht empfangen: {content}",
                        e.Content);
                }
                else
                {
                    SetSessionStatus(sessionStatus.Value);
                }
            }
        }

        private void SetSessionStatus(SessionStatusType newSessionStatus)
        {
            if (newSessionStatus != SessionStatus)
            {
                SessionStatus = newSessionStatus;

                switch (SessionStatus)
                {
                    case SessionStatusType.InPreparation:
                        logger?.LogInformation("Die Session wird vorbereitet.");
                        break;

                    case SessionStatusType.IsRunning:
                        logger?.LogInformation("Die Session wurde gestartet.");
                        break;

                    case SessionStatusType.IsEnded:
                        logger?.LogInformation("Die Session wurde beendet.");
                        break;

                    case SessionStatusType.IsPaused:
                        logger?.LogInformation("Die Session wird pausiert.");
                        break;
                }

                SessionChangedEvent?.Invoke(
                    sender: this,
                    e: new StateChangedArgs(SessionStatus));
            }
            else
            {
                logger?.LogDebug("Der Status der Session hat sich nicht geändert.");
            }
        }

        private async Task SetSessionStatusInitally()
        {
            var currentSession = await databaseConnector.GetEBuEfSessionAsync();

            if (currentSession != default)
            {
                SetSessionStatus(currentSession.Status);
            }
        }

        #endregion Private Methods
    }
}