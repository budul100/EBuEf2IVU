using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Commons.EventsArgs;
using Commons.Interfaces;
using MQTTnet;
using MQTTnet.Client;
using Polly;
using Polly.Retry;

namespace MQTTReceiver
{
    public class Receiver
        : IMQTTReceiver, IDisposable
    {
        #region Private Fields

        private readonly ILogger<Receiver> logger;
        private readonly IMqttClient mqttClient;
        private readonly MqttFactory mqttFactory;

        private bool isDisposed;
        private string messageType;
        private int? port;
        private Task receiverTask;
        private AsyncRetryPolicy retryPolicy;
        private string server;
        private string topic;

        #endregion Private Fields

        #region Public Constructors

        public Receiver(ILogger<Receiver> logger)
        {
            this.logger = logger;

            mqttFactory = new MqttFactory();

            mqttClient = mqttFactory.CreateMqttClient();
            mqttClient.ApplicationMessageReceivedAsync += e => Task.Run(() => SendMessageReceived(e.ApplicationMessage));
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler<MessageReceivedArgs> MessageReceivedEvent;

        #endregion Public Events

        #region Public Methods

        public void Dispose()
        {
            Dispose(
                isDisposing: true);

            GC.SuppressFinalize(
                obj: this);
        }

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

        public void Initialize(string server, int? port, string topic, int retryTime, string messageType)
        {
            this.server = server;
            this.port = port;
            this.topic = topic;
            this.messageType = messageType;

            retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    sleepDurationProvider: _ => TimeSpan.FromSeconds(retryTime),
                    onRetry: OnRetry);
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    StopTask();

                    mqttClient.Dispose();
                }

                isDisposed = true;
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void OnRetry(Exception exception, TimeSpan reconnection)
        {
            while (exception.InnerException != default) exception = exception.InnerException;

            if (exception.GetType() != typeof(OperationCanceledException))
            {
                var address = port.HasValue
                    ? $"{server}:{port}"
                    : server;

                logger.LogError(
                    exception,
                    "Fehler beim MQTT-Empfänger {address} (topic {topic}). {message}\r\n" +
                    "Die Verbindung wird in {reconnection} Sekunden wieder versucht.",
                    address,
                    topic,
                    exception.Message,
                    reconnection.TotalSeconds);
            }

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private async Task RunReceiverAsync(CancellationToken cancellationToken)
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(
                    server: server,
                    port: port).Build();

            await mqttClient.ConnectAsync(
                options: mqttClientOptions,
                cancellationToken: cancellationToken);

            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic(topic))
                .Build();

            await mqttClient.SubscribeAsync(
                options: mqttSubscribeOptions,
                cancellationToken: cancellationToken);

            var address = port.HasValue
                ? $"{server}:{port}"
                : server;

            logger.LogDebug(
                "MQTT receiver is subscribed on {address} (topic {topic}) for messages of type {type}.",
                address,
                topic,
                messageType);
        }

        private void SendMessageReceived(MqttApplicationMessage message)
        {
            if (message?.PayloadSegment.Array?.Length > 0)
            {
                var content = Encoding.ASCII.GetString(message.PayloadSegment.Array);

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