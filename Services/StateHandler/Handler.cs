using Common.EventsArgs;
using Common.Interfaces;
using MessageReceiver;
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

        private const string MessageType = "Sessionstatus";
        private const string StatusRegexGroupName = "status";
        private const string StatusRegexGroupWildcard = "$";

        private readonly ILogger logger;

        private IReceiver receiver;
        private Regex startRegex;
        private Regex statusRegex;

        #endregion Private Fields

        #region Public Constructors

        public Handler(ILogger<Handler> logger)
        {
            this.logger = logger;
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler<StateChangedArgs> SessionChangedEvent;

        public event EventHandler SessionStartedEvent;

        #endregion Public Events

        #region Public Methods

        public void Initialize(string ipAdress, int port, int retryTime, string startPattern, string statusPattern)
        {
            receiver = new Receiver(
                ipAdress: ipAdress,
                port: port,
                retryTime: retryTime,
                logger: logger,
                messageType: MessageType);
            receiver.MessageReceivedEvent += OnStatusReceived;

            startRegex = GetStartRegex(startPattern);
            statusRegex = GetStatusRegex(statusPattern);
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            return receiver.RunAsync(cancellationToken);
        }

        #endregion Public Methods

        #region Private Methods

        private static Regex GetStartRegex(string startPattern)
        {
            return new Regex(startPattern);
        }

        private Regex GetStatusRegex(string statusPattern)
        {
            var correctedPattern = statusPattern.Replace(
                oldValue: StatusRegexGroupWildcard,
                newValue: $@"(?<{StatusRegexGroupName}>\d)");

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
            else if (statusRegex.IsMatch(e.Content))
            {
                var sessionStatus = statusRegex
                    .Match(e.Content)
                    .Groups[StatusRegexGroupName].Value;

                SessionChangedEvent?.Invoke(
                    sender: this,
                    e: new StateChangedArgs(sessionStatus));
            }
            else
            {
                logger.LogError($"Unbekannte Status-Nachricht empfangen: '{e.Content}'.");
            }
        }

        #endregion Private Methods
    }
}