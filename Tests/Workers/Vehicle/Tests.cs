using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EBuEf2IVU.Shareds.Commons.Enums;
using EBuEf2IVU.Shareds.Commons.EventsArgs;
using EBuEf2IVU.Shareds.Commons.Extensions;
using EBuEf2IVU.Shareds.Commons.Interfaces;
using EBuEf2IVU.Shareds.Commons.Models;
using EBuEf2IVU.Shareds.TestsBase;
using Moq;
using Xunit;

namespace EBuEf2IVU.Shareds.VehicleWorkerTests
{
    public class Tests
        : Base
    {
        #region Private Fields

        private const string SettingsPath = @"..\..\..\Tests.xml";

        #endregion Private Fields

        #region Public Methods

        [Fact]
        public async Task InitializeSessionAfterRestart()
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

            using var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await host.StartAsync(cancellationTokenSource.Token);
                Assert.False(wasCalled);

                stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsRunning));
                Assert.True(wasCalled);

                wasCalled = false;

                stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsPaused));
                Assert.False(wasCalled);

                stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsRunning));
                Assert.True(wasCalled);
            }
            finally
            {
                await host.StopAsync(CancellationToken.None);
                host.Dispose();
            }
        }

        [Fact]
        public async Task SendMessageForDifferentDate()
        {
            //For testing the import on a local application server, the settings must be adjusted:
            //
            //<Division>[Set correct division]</Division>
            //<Host>[Set hostname of app server]</Host>

            var stateHandlerMock = GetStateHandlerMock();
            var multicastReceiver = new Mock<IMulticastReceiver>();
            var mqttReceiver = new Mock<IMQTTReceiver>();

            var testDate = new DateTime(2022, 8, 4);
            var databaseConnectorMock = GetDatabaseConnectorMock(
                ivuDatum: testDate);

            var settingsPath = Path.GetFullPath(SettingsPath);

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices(services => ConfigureServices(services, databaseConnectorMock, stateHandlerMock, multicastReceiver.Object, mqttReceiver.Object))
                .Build();

            using var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await host.StartAsync(cancellationTokenSource.Token);

                stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsRunning));

                const string messageContent = "{\"zugnummer\":\"13\",\"decoder\":null,\"simulationszeit\":\"1970-01-01 01:00:00\",\"betriebsstelle\":\"BGS\",\"signaltyp\":\"ESig\",\"start_gleis\":\"1\",\"ziel_gleis\":\"2\",\"modus\":\"istzeit\"}";
                multicastReceiver.Raise(m => m.MessageReceivedEvent += null, new MessageReceivedArgs(messageContent));

                await Task.Delay(1000, CancellationToken.None);
            }
            finally
            {
                await host.StopAsync(CancellationToken.None);
                host.Dispose();
            }
        }

        [Fact]
        public async Task UpdateRealtimeInDatabase()
        {
            var stateHandlerMock = GetStateHandlerMock();
            var multicastReceiver = new Mock<IMulticastReceiver>();
            var mqttReceiver = new Mock<IMQTTReceiver>();

            var legReceived = default(TrainLeg);

            var databaseConnectorMock = GetDatabaseConnectorMock(
                addRealtimeCallback: l => legReceived = l);

            var settingsPath = Path.GetFullPath(SettingsPath);

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices(services => ConfigureServices(services, databaseConnectorMock, stateHandlerMock, multicastReceiver.Object, mqttReceiver.Object))
                .Build();

            using var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await host.StartAsync(cancellationTokenSource.Token);

                stateHandlerMock.Raise(s => s.SessionChangedEvent += default, new StateChangedArgs(StateType.IsRunning));

                const string messageContent = "{\"zugnummer\":\"13\",\"decoder\":null,\"simulationszeit\":\"1970-01-01 01:00:00\",\"betriebsstelle\":\"BGS\",\"signaltyp\":\"ESig\",\"start_gleis\":\"1\",\"ziel_gleis\":\"2\",\"modus\":\"istzeit\"}";
                multicastReceiver.Raise(m => m.MessageReceivedEvent += null, new MessageReceivedArgs(messageContent));

                await Task.Delay(1000, CancellationToken.None);

                Assert.Equal("1", legReceived.EBuEfGleisVon);
                Assert.Equal("2", legReceived.EBuEfGleisNach);
                Assert.Equal("2", legReceived.IVUGleis);
            }
            finally
            {
                await host.StopAsync(CancellationToken.None);
                host.Dispose();
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureServices(IServiceCollection services, Mock<IDatabaseConnector> databaseConnectorMock,
            Mock<IStateHandler> stateHandlerMock, IMulticastReceiver multicastReceiver, IMQTTReceiver mQTTReceiver)
        {
            services.AddHostedService<Workers.Vehicle.Worker>();

            services.AddTransient<Mock<ILogger>>();
            services.AddSingleton(databaseConnectorMock.Object);
            services.AddSingleton(stateHandlerMock.Object);

            services.AddSingleton(multicastReceiver);
            services.AddSingleton(mQTTReceiver);
            services.AddSingleton<IMessage2LegConverter, Services.Message2LegConverter.Converter>();

            services.AddSingleton<IRealtimeSenderIS, Services.RealtimeSenderIS.Sender>();
            services.AddSingleton<IRealtimeSender, Services.RealtimeSender.Sender>();
        }

        #endregion Private Methods
    }
}