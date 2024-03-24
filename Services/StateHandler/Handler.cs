using Commons.Enums;
using Commons.EventsArgs;
using Commons.Extensions;
using Commons.Interfaces;
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
        private readonly IMQTTReceiver mqttReceiver;
        private readonly IMulticastReceiver multicastReceiver;

        private IMessageReceiver messageReceiver;
        private Regex sessionStartRegex;
        private Regex sessionStatusRegex;

        #endregion Private Fields

        #region Public Constructors

        public Handler(ILogger<Handler> logger, IDatabaseConnector databaseConnector,
            IMulticastReceiver multicastReceiver, IMQTTReceiver mqttReceiver)
        {
            this.logger = logger;
            this.databaseConnector = databaseConnector;

            this.multicastReceiver = multicastReceiver;
            this.multicastReceiver.MessageReceivedEvent += OnStatusReceived;

            this.mqttReceiver = mqttReceiver;
            this.mqttReceiver.MessageReceivedEvent += OnStatusReceived;
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler<StateChangedArgs> SessionChangedEvent;

        #endregion Public Events

        #region Public Properties

        public StateType StateType { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await SetSessionStatusInitally();

            await messageReceiver.ExecuteAsync(cancellationToken);
        }

        public void Initialize(string server, string topic, int retryTime, string sessionStartPattern,
            string sessionStatusPattern)
        {
            mqttReceiver.Initialize(
                server: server,
                topic: topic,
                retryTime: retryTime,
                messageType: MessageTypeState);

            messageReceiver = mqttReceiver;

            sessionStartRegex = sessionStartPattern.GetSessionStartRegex();
            sessionStatusRegex = sessionStatusPattern.GetSessionStatusRegex();
        }

        public void Initialize(string host, int port, int retryTime, string sessionStartPattern,
            string sessionStatusPattern)
        {
            multicastReceiver.Initialize(
                host: host,
                port: port,
                retryTime: retryTime,
                messageType: MessageTypeState);

            messageReceiver = multicastReceiver;

            sessionStartRegex = sessionStartPattern.GetSessionStartRegex();
            sessionStatusRegex = sessionStatusPattern.GetSessionStatusRegex();
        }

        #endregion Public Methods

        #region Private Methods

        private StateType? GetSessionStatus(string message)
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
                SetSessionStatus(StateType.IsRunning);
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

        private void SetSessionStatus(StateType stateType)
        {
            StateType = stateType;

            switch (StateType)
            {
                case StateType.InPreparation:
                    logger?.LogInformation("Die Session wird vorbereitet.");
                    break;

                case StateType.IsRunning:
                    logger?.LogInformation("Die Session wurde gestartet.");
                    break;

                case StateType.IsEnded:
                    logger?.LogInformation("Die Session wurde beendet.");
                    break;

                case StateType.IsPaused:
                    logger?.LogInformation("Die Session wird pausiert.");
                    break;
            }

            SessionChangedEvent?.Invoke(
                sender: this,
                e: new StateChangedArgs(StateType));
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