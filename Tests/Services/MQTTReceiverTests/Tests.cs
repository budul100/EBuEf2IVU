using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Commons.Extensions;
using Commons.Interfaces;
using EBuEf2IVUTestBase;
using Moq;
using MQTTnet;
using MQTTnet.Server;
using MQTTReceiver;

namespace MQTTReceiverTests
{
    public class Tests
        : TestsBase
    {
        #region Private Fields

        private const string MqttServer = "localhost";
        private const string MqttTopic = "samples/tests";
        private const string SettingsPath = @"..\..\..\MQTTReceiverTests.example.xml";

        #endregion Private Fields

        #region Public Methods

        [Test]
        public async Task SendMessage()
        {
            var serverFactory = new MqttServerFactory();

            var serverOptions = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .Build();

            using var server = serverFactory.CreateMqttServer(serverOptions);

            await server.StartAsync();

            var clientFactory = new MqttClientFactory();

            using var client = clientFactory.CreateMqttClient();

            var senderOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(MqttServer).Build();

            await client.ConnectAsync(senderOptions, CancellationToken.None);

            var hasException = false;

            var loggerMock = GetLoggerMock<Receiver>(() => hasException = true);

            var settingsPath = Path.GetFullPath(SettingsPath);

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices(services => ConfigureServices(services, loggerMock))
                .Build();

            const string payload = "testmessage";
            var messageReceivedTcs = new TaskCompletionSource<bool>();

            var receiver = host.Services.GetService<IMQTTReceiver>();
            receiver.MessageReceivedEvent += (s, e) =>
            {
                if (e.Content == payload)
                {
                    messageReceivedTcs.TrySetResult(true);
                }
            };

            receiver.Initialize(
                server: MqttServer,
                port: default,
                topic: MqttTopic,
                retryTime: 30,
                messageType: default);

            using var cts = new CancellationTokenSource();
            var receiverTask = receiver.ExecuteAsync(cts.Token);

            // Warte, bis der Receiver wirklich bereit ist
            await Task.Delay(2000);

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(MqttTopic)
                .WithPayload(payload)
                .Build();

            await client.PublishAsync(applicationMessage, CancellationToken.None);

            // Warte auf die Message mit Timeout
            var hasReceived = await Task.WhenAny(
                messageReceivedTcs.Task,
                Task.Delay(5000)
                                                ) == messageReceivedTcs.Task && messageReceivedTcs.Task.Result;

            await client.DisconnectAsync();

            // Beende den Receiver ordnungsgem‰ﬂ
            cts.Cancel();

            try
            {
                await Task.WhenAny(receiverTask, Task.Delay(2000));
            }
            catch
            { }

            await server.StopAsync();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(hasException, Is.False);
                Assert.That(hasReceived, Is.True);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureServices(IServiceCollection services, Mock<ILogger<Receiver>> loggerMock)
        {
            services.AddSingleton(loggerMock.Object);
            services.AddSingleton<IMQTTReceiver, Receiver>();
        }

        #endregion Private Methods
    }
}