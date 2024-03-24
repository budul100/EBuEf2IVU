using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Commons.EventsArgs;
using Commons.Interfaces;
using Polly;
using Polly.Retry;

namespace MulticastReceiver
{
    public class Receiver(ILogger<Receiver> logger)
        : IMulticastReceiver
    {
        #region Private Fields

        private const char EndChar = '\0';

        private readonly ILogger logger = logger;

        private string host;
        private string messageType;
        private int port;
        private Task receiverTask;
        private AsyncRetryPolicy retryPolicy;

        #endregion Private Fields

        #region Public Events

        public event EventHandler<MessageReceivedArgs> MessageReceivedEvent;

        #endregion Public Events

        #region Public Methods

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (receiverTask == default)
            {
                cancellationToken.Register(StopTask);

                receiverTask = retryPolicy.ExecuteAsync(
                    action: RunReceiverAsync,
                    cancellationToken: cancellationToken);
            }

            return receiverTask;
        }

        public void Initialize(string host, int port, int retryTime, string messageType)
        {
            this.host = host;
            this.port = port;
            this.messageType = messageType;

            retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    sleepDurationProvider: _ => TimeSpan.FromSeconds(retryTime),
                    onRetry: OnRetry);
        }

        #endregion Public Methods

        #region Private Methods

        private void OnRetry(Exception exception, TimeSpan reconnection)
        {
            if (exception.GetType() == typeof(SocketException))
            {
                var socketException = exception as SocketException;

                logger.LogError(
                    socketException,
                    "Fehler beim Nachrichtenempfänger an {address}: {message} (Code: {code})\r\n" +
                    "Die Verbindung wird in {reconnection} Sekunden wieder versucht.",
                    $"{host}:{port}",
                    socketException.SocketErrorCode,
                    exception.Message,
                    reconnection.TotalSeconds);
            }
            else
            {
                while (exception.InnerException != default) exception = exception.InnerException;

                if (exception.GetType() != typeof(OperationCanceledException))
                {
                    logger.LogError(
                        exception,
                        "Fehler beim Nachrichtenempfänger an {address}: {message}\r\n" +
                        "Die Verbindung wird in {reconnection} Sekunden wieder versucht.",
                        $"{host}:{port}",
                        exception.Message,
                        reconnection.TotalSeconds);
                }
            }
        }

        private async Task RunReceiverAsync(CancellationToken cancellationToken)
        {
            var multicastAddress = IPAddress.Parse(host);
            var localEp = new IPEndPoint(
                address: IPAddress.Any,
                port: port);

            using var udpClient = new UdpClient
            {
                ExclusiveAddressUse = false
            };

            udpClient.Client.SetSocketOption(
                optionLevel: SocketOptionLevel.Socket,
                optionName: SocketOptionName.ReuseAddress,
                optionValue: true);
            udpClient.Client.Bind(localEp);
            udpClient.JoinMulticastGroup(multicastAddress);

            logger.LogDebug(
                "Lausche auf {address} nach Nachrichten vom Typ {type}.",
                $"{host}:{port}",
                messageType);

            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await udpClient.ReceiveAsync(cancellationToken);
                SendMessageReceived(result);
            }
        }

        private void SendMessageReceived(UdpReceiveResult result)
        {
            var bytes = result.Buffer;

            if (bytes?.Length > 0)
            {
                var content = Encoding.ASCII.GetString(
                    bytes: [.. bytes],
                    index: 0,
                    count: bytes.Length).TrimEnd(EndChar);

                if (!string.IsNullOrWhiteSpace(content))
                {
                    MessageReceivedEvent?.Invoke(
                        sender: this,
                        e: new MessageReceivedArgs(content));
                }
            }
        }

        private void StopTask()
        {
            receiverTask = default;
        }

        #endregion Private Methods
    }
}