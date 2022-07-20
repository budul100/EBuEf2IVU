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

namespace EBuEf2IVUVehicleTests
{
    public class Tests
        : TestsBase
    {
        #region Public Methods

        [Test]
        public void InitializeSessionAfterRestart()
        {
            var wasCalled = false;

            var cancellationTokenSource = new CancellationTokenSource();

            var databaseConnectorMock = GetDatabaseConnectorMock(
                getEbuefSession: () => wasCalled = true);
            var stateHandlerMock = GetStateHandlerMock();

            var settingsPath = Path.GetFullPath(@"..\..\..\..\..\..\Programs\EBuEf2IVUVehicle\ebuef2ivuvehicle-settings.example.xml");

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices(services => ConfigureServices(services, databaseConnectorMock, stateHandlerMock))
                .Build();

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
            Mock<IStateHandler> stateHandlerMock)
        {
            services.AddHostedService<EBuEf2IVUVehicle.Worker>();

            services.AddTransient<Mock<ILogger>>();
            services.AddSingleton(databaseConnectorMock.Object);
            services.AddSingleton(stateHandlerMock.Object);

            services.AddSingleton((new Mock<IMessageReceiver>()).Object);
            services.AddSingleton((new Mock<IRealtimeSender>()).Object);
            services.AddSingleton((new Mock<IRealtimeSenderIS>()).Object);
        }

        #endregion Private Methods
    }
}