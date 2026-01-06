using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EBuEf2IVU.Services.MQTTReceiver;
using EBuEf2IVU.Shareds.Commons.Extensions;
using EBuEf2IVU.Shareds.Commons.Interfaces;
using EBuEf2IVU.Shareds.TestsBase;
using Moq;
using MQTTnet;
using MQTTnet.Server;

namespace EBuEf2IVU.Shareds.MQTTReceiverTests
{
    public class Tests
        : Base
    {
        #region Private Fields

        private const string MqttServer = "localhost";
        private const string MqttTopic = "samples/tests";
        private const string SettingsPath = @"..\..\..\Tests.xml";

        #endregion Private Fields

        #region Public Methods

        [Fact]
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
            var completedTask = await Task.WhenAny(
                messageReceivedTcs.Task,
                Task.Delay(5000));

            var hasReceived = completedTask == messageReceivedTcs.Task
                && await messageReceivedTcs.Task;

            await client.DisconnectAsync();

            // Beende den Receiver ordnungsgem‰ﬂ
            cts.Cancel();

            try
            {
                await Task.WhenAny(
                    receiverTask,
                    Task.Delay(2000));
            }
            catch
            { }

            await server.StopAsync();

            Assert.False(hasException);
            Assert.True(hasReceived);
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