using Polly;
using Polly.Retry;
using Serilog;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MessageReceiver.Internals
{
    internal class Receiver
    {
        #region Private Fields

        private const char EndChar = '\0';

        private readonly string ipAdress;
        private readonly ILogger logger;
        private readonly int port;
        private readonly RetryPolicy retryPolicy;

        #endregion Private Fields

        #region Public Constructors

        public Receiver(string ipAdress, int port, int reconnectionTime, ILogger logger)
        {
            this.ipAdress = ipAdress;
            this.port = port;
            this.logger = logger;

            retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryForever(
                    sleepDurationProvider: (p) => TimeSpan.FromSeconds(reconnectionTime),
                    onRetry: (exception, reconnection) => OnRetry(exception, reconnection));
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler<MessageReceivedArgs> MessageReceivedEvent;

        #endregion Public Events

        #region Public Methods

        public void Run(CancellationToken cancellationToken)
        {
            retryPolicy.Execute(
                action: (t) => RunReceiver(t),
                cancellationToken: cancellationToken);
        }

        #endregion Public Methods

        #region Private Methods

        private void OnRetry(Exception exception, TimeSpan reconnection)
        {
            if (exception.GetType() == typeof(SocketException))
            {
                var socketException = exception as SocketException;

                logger.Error(
                    $"Fehler beim Nachrichtenempfänger an {ipAdress}:{port} " +
                    $"(Code: {socketException.SocketErrorCode}): {exception.Message}\r\n" +
                    $"Die Verbindung wird in {reconnection.TotalSeconds} Sekunden wieder versucht.");
            }
            else
            {
                logger.Error(
                    $"Fehler beim Nachrichtenempfänger an {ipAdress}:{port}: {exception.Message}\r\n" +
                    $"Die Verbindung wird in {reconnection.TotalSeconds} Sekunden wieder versucht.");
            }
        }

        private void RunReceiver(CancellationToken cancellationToken)
        {
            var multicastAddress = IPAddress.Parse(ipAdress);
            var localEp = new IPEndPoint(
                address: IPAddress.Any,
                port: port);

            using (var client = new UdpClient())
            {
                client.ExclusiveAddressUse = false;
                client.Client.SetSocketOption(
                    optionLevel: SocketOptionLevel.Socket,
                    optionName: SocketOptionName.ReuseAddress,
                    optionValue: true);
                client.Client.Bind(localEp);
                client.JoinMulticastGroup(multicastAddress);

                logger.Debug($"Lausche auf {ipAdress}:{port} nach Nachrichten.");

                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = client.Receive(ref localEp);

                    var content = Encoding.ASCII.GetString(
                        bytes: result,
                        index: 0,
                        count: result.Length).TrimEnd(EndChar);

                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        var messageReceivedArgs = new MessageReceivedArgs
                        {
                            Content = content,
                        };

                        MessageReceivedEvent?.Invoke(
                            sender: this,
                            e: messageReceivedArgs);
                    }
                }
            }
        }

        #endregion Private Methods
    }
}