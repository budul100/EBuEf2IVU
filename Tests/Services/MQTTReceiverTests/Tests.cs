using Commons.Extensions;
using Commons.Interfaces;
using EBuEf2IVUTestBase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using MQTTReceiver;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
            var mqttFactory = new MqttFactory();

            var serverOptions = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .Build();

            using var server = mqttFactory.CreateMqttServer(serverOptions);

            await server.StartAsync();

            using var sender = mqttFactory.CreateMqttClient();

            var senderOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(
                    server: MqttServer).Build();

            await sender.ConnectAsync(senderOptions, CancellationToken.None);

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

            Task.Run(() => receiver.ExecuteAsync(CancellationToken.None));

            Thread.Sleep(5000);

            await sender.PublishAsync(applicationMessage, CancellationToken.None);

            await sender.DisconnectAsync();

            await server.StopAsync();

            Assert.That(hasException, Is.False);
            Assert.That(hasReceived, Is.True);
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