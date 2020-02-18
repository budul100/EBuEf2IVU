using Common.EventsArgs;
using Common.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageReceiver
{
    public class Receiver
        : IMessageReceiver
    {
        #region Private Fields

        private const char EndChar = '\0';

        private readonly string ipAdress;
        private readonly ILogger logger;
        private readonly string messageType;
        private readonly int port;
        private readonly AsyncRetryPolicy retryPolicy;

        #endregion Private Fields

        #region Public Constructors

        public Receiver(ILogger logger, string ipAdress, int port, int retryTime, string messageType = default)
        {
            this.ipAdress = ipAdress;
            this.port = port;
            this.logger = logger;
            this.messageType = messageType;

            retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    sleepDurationProvider: (p) => TimeSpan.FromSeconds(retryTime),
                    onRetry: (exception, reconnection) => OnRetry(
                        exception: exception,
                        reconnection: reconnection));
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler<MessageReceivedArgs> MessageReceivedEvent;

        #endregion Public Events

        #region Public Methods

        public Task RunAsync(CancellationToken cancellationToken)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (t) => RunReceiverAsync(t),
                cancellationToken: cancellationToken);

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private void OnRetry(Exception exception, TimeSpan reconnection)
        {
            if (exception.GetType() == typeof(SocketException))
            {
                var socketException = exception as SocketException;

                logger.LogError(
                    $"Fehler beim Nachrichtenempfänger an {ipAdress}:{port} " +
                    $"(Code: {socketException.SocketErrorCode}): {exception.Message}\r\n" +
                    $"Die Verbindung wird in {reconnection.TotalSeconds} Sekunden wieder versucht.");
            }
            else
            {
                while (exception.InnerException != null) exception = exception.InnerException;

                logger.LogError(
                    $"Fehler beim Nachrichtenempfänger an {ipAdress}:{port}: {exception.Message}\r\n" +
                    $"Die Verbindung wird in {reconnection.TotalSeconds} Sekunden wieder versucht.");
            }
        }

        private async Task RunReceiverAsync(CancellationToken cancellationToken)
        {
            var multicastAddress = IPAddress.Parse(ipAdress);
            var localEp = new IPEndPoint(
                address: IPAddress.Any,
                port: port);

            using (var udpClient = new UdpClient())
            {
                udpClient.ExclusiveAddressUse = false;
                udpClient.Client.SetSocketOption(
                    optionLevel: SocketOptionLevel.Socket,
                    optionName: SocketOptionName.ReuseAddress,
                    optionValue: true);
                udpClient.Client.Bind(localEp);
                udpClient.JoinMulticastGroup(multicastAddress);

                logger.LogDebug($"Lausche auf {ipAdress}:{port} nach Nachrichten" +
                    (!string.IsNullOrWhiteSpace(messageType) ? $" vom Typ {messageType}." : "."));

                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = await udpClient.ReceiveAsync();
                    SendMessageReceived(result);
                }
            }
        }

        private void SendMessageReceived(UdpReceiveResult result)
        {
            var bytes = result.Buffer;

            if (bytes?.Any() ?? false)
            {
                var content = Encoding.ASCII.GetString(
                    bytes: bytes.ToArray(),
                    index: 0,
                    count: bytes.Count()).TrimEnd(EndChar);

                if (!string.IsNullOrWhiteSpace(content))
                {
                    MessageReceivedEvent?.Invoke(
                        sender: this,
                        e: new MessageReceivedArgs(content));
                }
            }
        }

        #endregion Private Methods
    }
}