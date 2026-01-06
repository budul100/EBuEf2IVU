using System.IO;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EBuEf2IVU.Shareds.Commons.Enums;
using EBuEf2IVU.Shareds.Commons.EventsArgs;
using EBuEf2IVU.Shareds.Commons.Extensions;
using EBuEf2IVU.Shareds.Commons.Interfaces;
using EBuEf2IVU.Shareds.TestsBase;
using Moq;

namespace EBuEf2IVU.Shareds.PathWorkerTests
{
    public class Tests
        : Base
    {
        #region Private Fields

        private const string SettingsPath = @"..\..\..\Tests.xml";

        #endregion Private Fields

        #region Public Methods

        [Fact]
        public void InitializeSessionAfterRestart()
        {
            var wasCalled = false;

            var stateHandlerMock = GetStateHandlerMock();
            var multicastReceiver = new Mock<IMulticastReceiver>();
            var mqttReceiver = new Mock<IMQTTReceiver>();

            var databaseConnectorMock = GetDatabaseConnectorMock(
                sessionCallback: () => wasCalled = true);

            var settingsPath = Path.GetFullPath(SettingsPath);

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices(services => ConfigureServices(services, databaseConnectorMock, stateHandlerMock, multicastReceiver.Object, mqttReceiver.Object))
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

        [Fact]
        public void InitializeSessionTwice()
        {
            var stateHandlerMock = GetStateHandlerMock();
            var multicastReceiver = new Mock<IMulticastReceiver>();
            var mqttReceiver = new Mock<IMQTTReceiver>();

            var eventCount = 0;

            var databaseConnectorMock = GetDatabaseConnectorMock(
                sendTrainRuns: true,
                trainRunsPlanCallback: () => eventCount++);

            var settingsPath = Path.GetFullPath(SettingsPath);

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices(services => ConfigureServices(services, databaseConnectorMock, stateHandlerMock, multicastReceiver.Object, mqttReceiver.Object))
                .Build();

            var cancellationTokenSource = new CancellationTokenSource();

            host.StartAsync(cancellationTokenSource.Token);
            Assert.Equal(0, eventCount);

            Thread.Sleep(500);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.InPreparation));
            Assert.Equal(1, eventCount);

            Thread.Sleep(500);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.InPreparation));
            Assert.Equal(1, eventCount);

            Thread.Sleep(500);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsEnded));
            Assert.Equal(1, eventCount);

            Thread.Sleep(500);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.InPreparation));
            Assert.Equal(2, eventCount);

            Thread.Sleep(500);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsRunning));
            Assert.Equal(2, eventCount);

            Thread.Sleep(500);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsPaused));
            Assert.Equal(2, eventCount);

            Thread.Sleep(500);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsRunning));
            Assert.Equal(2, eventCount);

            Thread.Sleep(500);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsEnded));
            Assert.Equal(2, eventCount);

            Thread.Sleep(500);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsRunning));
            Assert.Equal(3, eventCount);
        }

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureServices(IServiceCollection services, Mock<IDatabaseConnector> databaseConnectorMock,
            Mock<IStateHandler> stateHandlerMock, IMulticastReceiver multicastReceiver, IMQTTReceiver mQTTReceiver)
        {
            services.AddHostedService<Workers.Path.Worker>();

            services.AddTransient<Mock<ILogger>>();
            services.AddSingleton(databaseConnectorMock.Object);
            services.AddSingleton(stateHandlerMock.Object);

            services.AddSingleton(multicastReceiver);
            services.AddSingleton(mQTTReceiver);
            services.AddSingleton<IMessage2TrainRunConverter, Services.Message2TrainRunConverter.Converter>();

            services.AddSingleton<ITrainPathSender, Services.TrainPathSender.Sender>();
        }

        #endregion Private Methods
    }
}