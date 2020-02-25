using Common.Enums;
using Common.EventsArgs;
using Common.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace StateHandler
{
    public class Handler
        : IStateHandler
    {
        #region Private Fields

        private const string MessageType = "Sessionstatus";
        private const string StatusRegexGroupName = "status";
        private const string StatusRegexGroupWildcard = "$";

        private readonly ILogger logger;
        private readonly IMessageReceiver stateReceiver;

        private Regex startRegex;
        private Regex statusRegex;

        #endregion Private Fields

        #region Public Constructors

        public Handler(ILogger<Handler> logger, IMessageReceiver stateReceiver)
        {
            this.logger = logger;

            this.stateReceiver = stateReceiver;
            this.stateReceiver.MessageReceivedEvent += OnStatusReceived;
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler<StateChangedArgs> SessionChangedEvent;

        public event EventHandler SessionStartedEvent;

        #endregion Public Events

        #region Public Methods

        public void Initialize(string ipAdress, int port, int retryTime, string startPattern, string statusPattern)
        {
            stateReceiver.Initialize(
                ipAdress: ipAdress,
                port: port,
                retryTime: retryTime,
                messageType: MessageType);

            startRegex = GetStartRegex(startPattern);
            statusRegex = GetStatusRegex(statusPattern);
        }

        public void Run(CancellationToken cancellationToken)
        {
            stateReceiver.RunAsync(cancellationToken);
        }

        #endregion Public Methods

        #region Private Methods

        private static Regex GetStartRegex(string startPattern)
        {
            return new Regex(startPattern);
        }

        private Regex GetStatusRegex(string statusPattern)
        {
            // The new value cannot be created with \d!

            var correctedPattern = statusPattern.Replace(
                oldValue: StatusRegexGroupWildcard,
                newValue: @$"(?<{StatusRegexGroupName}>[0-9])");

            var result = new Regex(correctedPattern);

            return result;
        }

        private void OnStatusReceived(object sender, MessageReceivedArgs e)
        {
            logger?.LogDebug($"Status-Nachricht empfangen: {e.Content}");

            if (startRegex.IsMatch(e.Content))
            {
                SessionStartedEvent?.Invoke(
                    sender: this,
                    e: null);
            }
            else
            {
                var sessionStatus = statusRegex.IsMatch(e.Content)
                    ? statusRegex.Match(e.Content).Groups[StatusRegexGroupName].Value
                    : default;

                if (sessionStatus == SessionStates.InPreparation.ToString("D"))
                {
                    SessionChangedEvent?.Invoke(
                        sender: this,
                        e: new StateChangedArgs(SessionStates.InPreparation));
                }
                else if (sessionStatus == SessionStates.IsRunning.ToString("D"))
                {
                    SessionChangedEvent?.Invoke(
                        sender: this,
                        e: new StateChangedArgs(SessionStates.IsRunning));
                }
                else if (sessionStatus == SessionStates.IsEnded.ToString("D"))
                {
                    SessionChangedEvent?.Invoke(
                        sender: this,
                        e: new StateChangedArgs(SessionStates.IsEnded));
                }
                else if (sessionStatus == SessionStates.IsPaused.ToString("D"))
                {
                    SessionChangedEvent?.Invoke(
                        sender: this,
                        e: new StateChangedArgs(SessionStates.IsPaused));
                }
                else
                {
                    logger.LogError($"Unbekannte Status-Nachricht empfangen: '{e.Content}'.");
                }
            }
        }

        #endregion Private Methods
    }
}