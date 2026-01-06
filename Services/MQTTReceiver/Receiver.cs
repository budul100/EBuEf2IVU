using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using EBuEf2IVU.Shareds.Commons.EventsArgs;
using EBuEf2IVU.Shareds.Commons.Interfaces;
using MQTTnet;
using Polly;
using Polly.Retry;
using StringExtensions;

namespace EBuEf2IVU.Services.MQTTReceiver
{
    public class Receiver
        : IMQTTReceiver, IDisposable
    {
        #region Private Fields

        private readonly ILogger<Receiver> logger;
        private readonly IMqttClient mqttClient;
        private readonly MqttClientFactory mqttFactory;

        private string address;
        private bool isDisposed;
        private string messageType;
        private MqttClientOptions mqttClientOptions;
        private Task receiverTask;
        private AsyncRetryPolicy retryPolicy;
        private string topic;

        #endregion Private Fields

        #region Public Constructors

        public Receiver(ILogger<Receiver> logger)
        {
            this.logger = logger;

            mqttFactory = new MqttClientFactory();

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

        public void Initialize(string host, int? port, string topic, int? retryTime, string messageType)
        {
            this.topic = topic;
            this.messageType = messageType;

            address = port.HasValue
                ? $"{host}:{port}"
                : host;

            mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(
                    host: host,
                    port: port).Build();

            if (retryTime.HasValue)
            {
                retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryForeverAsync(
                        sleepDurationProvider: _ => TimeSpan.FromSeconds(retryTime.Value),
                        onRetry: OnRetry);
            }
            else
            {
                retryPolicy = Policy
                    .Handle<Exception>()
                    .RetryAsync(0);
            }
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
            await mqttClient.ConnectAsync(
                options: mqttClientOptions,
                cancellationToken: cancellationToken);

            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic(topic))
                .Build();

            await mqttClient.SubscribeAsync(
                options: mqttSubscribeOptions,
                cancellationToken: cancellationToken);

            logger.LogDebug(
                "MQTT receiver is subscribed on {address} (topic {topic}) for messages of type {type}.",
                address,
                topic,
                messageType);
        }

        private void SendMessageReceived(MqttApplicationMessage message)
        {
            var content = message?.ConvertPayloadToString();

            if (!content.IsEmpty())
            {
                MessageReceivedEvent?.Invoke(
                    sender: this,
                    e: new MessageReceivedArgs(content));
            }
        }

        private void StopTask()
        {
            receiverTask = default;
        }

        #endregion Private Methods
    }
}