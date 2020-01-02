using Common.Models;
using Common.EventsArgs;
using Common.Interfaces;
using MessageReceiver.Internals;
using MessageReceiver.Models;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MessageReceiver
{
    public class ReceiverManager : IReceiverManager
    {
        #region Private Fields

        private const string MessageTypeAllocations = "Sessionstart";
        private const string MessageTypeMessages = "Echtzeit-Positionen";

        private readonly CancellationToken cancellationToken;
        private readonly ILogger logger;
        private readonly JsonSerializerSettings realTimeMessageSettings;
        private Regex sessionStartRegex;

        #endregion Private Fields

        #region Public Constructors

        public ReceiverManager(ILogger logger, CancellationToken cancellationToken)
        {
            this.logger = logger;
            this.cancellationToken = cancellationToken;

            realTimeMessageSettings = new JsonSerializerSettings();
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler<RealTimeReceivedArgs> RealTimeReceivedEvent;

        public event EventHandler SessionStartedEvent;

        #endregion Public Events

        #region Public Methods

        public void Run(string allocationsIpAdress, int allocationsListenerPort, int allocationsRetryTime, string allocationsPattern,
            string messagesIpAdress, int messagesListenerPort, int messagesRetryTime)
        {
            sessionStartRegex = new Regex(allocationsPattern);

            Task.Run(() => RunAllocationsListener(
                ipAdress: allocationsIpAdress,
                port: allocationsListenerPort,
                retryTime: allocationsRetryTime));

            Task.Run(() => RunMessagesListener(
                ipAdress: messagesIpAdress,
                port: messagesListenerPort,
                retryTime: messagesRetryTime));
        }

        #endregion Public Methods

        #region Private Methods

        private void OnRealTimeReceived(object sender, MessageReceivedArgs e)
        {
            var message = default(RealTimeMessage);

            try
            {
                logger.Debug($"Message received: {e.Content}");

                message = JsonConvert.DeserializeObject<RealTimeMessage>(
                    value: e.Content,
                    settings: realTimeMessageSettings);
            }
            catch (JsonReaderException readerException)
            {
                logger.Error($"Received message cannot be converted into a real-time message: {readerException.Message}");
            }

            if (!string.IsNullOrWhiteSpace(message?.Zugnummer))
            {
                var args = new RealTimeReceivedArgs
                {
                    RealTimeMessage = message,
                };

                RealTimeReceivedEvent?.Invoke(
                    sender: this,
                    e: args);
            }
        }

        private void OnSessionStartReceived(object sender, MessageReceivedArgs e)
        {
            if (sessionStartRegex.IsMatch(e.Content))
            {
                SessionStartedEvent?.Invoke(
                    sender: this,
                    e: null);
            }
            else
            {
                logger.Error($"Unknown session start command received: '{e.Content}'.");
            }
        }

        private void RunAllocationsListener(string ipAdress, int port, int retryTime)
        {
            var receiver = new Receiver(
                ipAdress: ipAdress,
                port: port,
                reconnectionTime: retryTime,
                logger: logger,
                messageType: MessageTypeAllocations);

            receiver.MessageReceivedEvent += OnSessionStartReceived;

            receiver.Run(cancellationToken);
        }

        private void RunMessagesListener(string ipAdress, int port, int retryTime)
        {
            var receiver = new Receiver(
                ipAdress: ipAdress,
                port: port,
                reconnectionTime: retryTime,
                logger: logger,
                messageType: MessageTypeMessages);

            receiver.MessageReceivedEvent += OnRealTimeReceived;

            receiver.Run(cancellationToken);
        }

        #endregion Private Methods
    }
}