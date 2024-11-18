using System.IO;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Commons.Enums;
using Commons.EventsArgs;
using Commons.Extensions;
using Commons.Interfaces;
using EBuEf2IVUTestBase;
using Moq;
using NUnit.Framework;

namespace EBuEf2IVUPathTests
{
    public class Tests
        : TestsBase
    {
        #region Private Fields

        private const string SettingsPath = @"..\..\..\EBuEf2IVUPathTests.example.xml";

        #endregion Private Fields

        #region Public Methods

        [Test]
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
            Assert.That(wasCalled, Is.False);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsRunning));
            Assert.That(wasCalled, Is.True);

            wasCalled = false;

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsPaused));
            Assert.That(wasCalled, Is.False);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsRunning));
            Assert.That(wasCalled, Is.True);
        }

        [Test]
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
            Assert.That(eventCount, Is.EqualTo(0));

            Thread.Sleep(500);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.InPreparation));
            Assert.That(eventCount, Is.EqualTo(1));

            Thread.Sleep(500);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.InPreparation));
            Assert.That(eventCount, Is.EqualTo(1));

            Thread.Sleep(500);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsEnded));
            Assert.That(eventCount, Is.EqualTo(1));

            Thread.Sleep(500);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.InPreparation));
            Assert.That(eventCount, Is.EqualTo(2));

            Thread.Sleep(500);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsRunning));
            Assert.That(eventCount, Is.EqualTo(2));

            Thread.Sleep(500);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsPaused));
            Assert.That(eventCount, Is.EqualTo(2));

            Thread.Sleep(500);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsRunning));
            Assert.That(eventCount, Is.EqualTo(2));

            Thread.Sleep(500);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsEnded));
            Assert.That(eventCount, Is.EqualTo(2));

            Thread.Sleep(500);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsRunning));
            Assert.That(eventCount, Is.EqualTo(3));
        }

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureServices(IServiceCollection services, Mock<IDatabaseConnector> databaseConnectorMock,
            Mock<IStateHandler> stateHandlerMock, IMulticastReceiver multicastReceiver, IMQTTReceiver mQTTReceiver)
        {
            services.AddHostedService<EBuEf2IVUPath.Worker>();

            services.AddTransient<Mock<ILogger>>();
            services.AddSingleton(databaseConnectorMock.Object);
            services.AddSingleton(stateHandlerMock.Object);

            services.AddSingleton(multicastReceiver);
            services.AddSingleton(mQTTReceiver);
            services.AddSingleton<IMessage2TrainRunConverter, Message2TrainRunConverter.Converter>();

            services.AddSingleton<ITrainPathSender, TrainPathSender.Sender>();
        }

        #endregion Private Methods
    }
}