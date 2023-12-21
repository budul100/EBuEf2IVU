using Commons.Enums;
using Commons.EventsArgs;
using Commons.Extensions;
using Commons.Interfaces;
using Commons.Models;
using EBuEf2IVUTestBase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;

namespace EBuEf2IVUVehicleTests
{
    public class Tests
        : TestsBase
    {
        #region Private Fields

        private const string SettingsPath = @"..\..\..\EBuEf2IVUVehicleTests.example.xml";

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
                .ConfigureServices(services => ConfigureServices(services, messageReceiverMock.Object, databaseConnectorMock, stateHandlerMock))
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

        [Test]
        public void SendMessageForDifferentDate()
        {
            //For testing the import on a local application server, the settings must be adjusted:
            //
            //<Division>[Set correct division]</Division>
            //<Host>[Set hostname of app server]</Host>

            var testDate = new DateTime(2022, 8, 4);

            var databaseConnectorMock = GetDatabaseConnectorMock(
                ivuDatum: testDate);
            var stateHandlerMock = GetStateHandlerMock();
            var messageReceiverMock = new Mock<IMessageReceiver>();

            var settingsPath = Path.GetFullPath(SettingsPath);

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices(services => ConfigureServices(services, messageReceiverMock.Object, databaseConnectorMock, stateHandlerMock))
                .Build();

            var cancellationTokenSource = new CancellationTokenSource();
            host.StartAsync(cancellationTokenSource.Token);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsRunning));

            const string messageContent = "{\"zugnummer\":\"13\",\"decoder\":null,\"simulationszeit\":\"1970-01-01 01:00:00\",\"betriebsstelle\":\"BGS\",\"signaltyp\":\"ESig\",\"start_gleis\":\"1\",\"ziel_gleis\":\"2\",\"modus\":\"istzeit\"}";
            messageReceiverMock.Raise(m => m.MessageReceivedEvent += null, new MessageReceivedArgs(messageContent));

            Thread.Sleep(1000);
        }

        [Test]
        public void UpdateRealtimeInDatabase()
        {
            var legReceived = default(TrainLeg);

            var databaseConnectorMock = GetDatabaseConnectorMock(
                addRealtimeCallback: l => legReceived = l);

            var stateHandlerMock = GetStateHandlerMock();
            var messageReceiverMock = new Mock<IMessageReceiver>();

            var settingsPath = Path.GetFullPath(SettingsPath);

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices(services => ConfigureServices(services, messageReceiverMock.Object, databaseConnectorMock, stateHandlerMock))
                .Build();

            var cancellationTokenSource = new CancellationTokenSource();
            host.StartAsync(cancellationTokenSource.Token);

            stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsRunning));

            const string messageContent = "{\"zugnummer\":\"13\",\"decoder\":null,\"simulationszeit\":\"1970-01-01 01:00:00\",\"betriebsstelle\":\"BGS\",\"signaltyp\":\"ESig\",\"start_gleis\":\"1\",\"ziel_gleis\":\"2\",\"modus\":\"istzeit\"}";
            messageReceiverMock.Raise(m => m.MessageReceivedEvent += null, new MessageReceivedArgs(messageContent));

            Thread.Sleep(1000);

            Assert.True(legReceived.EBuEfGleisVon == "1");
            Assert.True(legReceived.EBuEfGleisNach == "2");

            Assert.True(legReceived.IVUGleis == "2");
        }

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureServices(IServiceCollection services, IMessageReceiver messageReceiver,
            Mock<IDatabaseConnector> databaseConnectorMock, Mock<IStateHandler> stateHandlerMock)
        {
            services.AddHostedService<EBuEf2IVUVehicle.Worker>();

            services.AddTransient<Mock<ILogger>>();
            services.AddSingleton(databaseConnectorMock.Object);
            services.AddSingleton(stateHandlerMock.Object);
            services.AddSingleton(messageReceiver);
            services.AddSingleton<IMessage2LegConverter, Message2LegConverter.Converter>();

            services.AddSingleton<IRealtimeSenderIS, RealtimeSenderIS.Sender>();
            services.AddSingleton<IRealtimeSender, RealtimeSender.Sender>();
        }

        #endregion Private Methods
    }
}