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
            var hasReceived = false;

            var receiver = host.Services.GetService<IMQTTReceiver>();
            receiver.MessageReceivedEvent += (s, e) => hasReceived = e.Content == payload;

            receiver.Initialize(
                server: MqttServer,
                port: default,
                topic: MqttTopic,
                retryTime: 30,
                messageType: default);

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(MqttTopic)
                .WithPayload(payload)
                .Build();

            var receiverTask = receiver.ExecuteAsync(CancellationToken.None);

            Thread.Sleep(5000);

            await client.PublishAsync(applicationMessage, CancellationToken.None);

            await client.DisconnectAsync();

            await server.StopAsync();

            await receiverTask;

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