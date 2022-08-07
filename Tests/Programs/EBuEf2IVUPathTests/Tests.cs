using Common.Enums;
using Common.EventsArgs;
using Common.Extensions;
using Common.Interfaces;
using EBuEf2IVUTestBase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Threading;

namespace EBuEf2IVUPathTests
{
    public class Tests
        : TestsBase
    {
        #region Private Fields

        private const string SettingsPath = @"..\..\..\..\..\..\Programs\EBuEf2IVUPath\ebuef2ivupath-settings.example.xml";

        #endregion Private Fields

        #region Public Methods

        [Test]
        public void InitializeSessionAfterRestart()
        {
            var wasCalled = false;

            var databaseConnectorMock = GetDatabaseConnectorMock(
                sessionCallback: () => wasCalled = true);
            var stateHandlerMock = GetStateHandlerMock();
            var messageReceiverMock = new Mock<IMessageReceiver>();

            var settingsPath = Path.GetFullPath(SettingsPath);

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices(services => ConfigureServices(services, databaseConnectorMock, stateHandlerMock, messageReceiverMock))
                .Build();

            var cancellationTokenSource = new CancellationTokenSource();

            host.StartAsync(cancellationTokenSource.Token);
            Assert.False(wasCalled);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsRunning));
            Assert.True(wasCalled);

            wasCalled = false;

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsPaused));
            Assert.False(wasCalled);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsRunning));
            Assert.True(wasCalled);
        }

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureServices(IServiceCollection services, Mock<IDatabaseConnector> databaseConnectorMock,
            Mock<IStateHandler> stateHandlerMock, Mock<IMessageReceiver> messageReceiverMock)
        {
            services.AddHostedService<EBuEf2IVUPath.Worker>();

            services.AddTransient<Mock<ILogger>>();
            services.AddSingleton(databaseConnectorMock.Object);
            services.AddSingleton(stateHandlerMock.Object);

            services.AddSingleton(messageReceiverMock.Object);
            services.AddSingleton<IMessage2TrainRunConverter, Message2TrainRunConverter.Converter>();

            services.AddSingleton<ITrainPathSender, TrainPathSender.Sender>();
        }

        #endregion Private Methods
    }
}