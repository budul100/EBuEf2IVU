using System;
using System.Collections.Generic;
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

namespace EBuEf2IVU.Shareds.CrewWorkerTests
{
    public class Tests
        : Base
    {
        #region Private Fields

        private const string SettingsPath = @"..\..\..\..\..\..\Shareds\Host\Settings\settings.crew.local.xml";

        #endregion Private Fields

        #region Public Methods

        [Fact]
        public async Task CheckCrewsOnRunningSessions()
        {
            var wasCalled = false;

            var databaseConnectorMock = GetDatabaseConnectorMock(
                sendTrainRuns: true);
            var stateHandlerMock = GetStateHandlerMock(
                sessionStatusType: StateType.IsRunning);
            var crewCheckerMock = GetCrewCheckerMock(
                getCrewingElements: () => wasCalled = true);

            var settingsPath = Path.GetFullPath(SettingsPath);

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices(services => ConfigureServices(services, databaseConnectorMock, stateHandlerMock, crewCheckerMock))
                .Build();

            using var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await host.StartAsync(cancellationTokenSource.Token);

                // Warte auf die asynchrone Initialisierung
                await Task.Delay(1000);

                Assert.True(wasCalled);
            }
            finally
            {
                cancellationTokenSource.Cancel();
                await host.StopAsync(CancellationToken.None);
                host.Dispose();
            }
        }

        [Fact]
        public void InitializeSessionAfterRestart()
        {
            var wasCalled = false;

            var cancellationTokenSource = new CancellationTokenSource();

            var databaseConnectorMock = GetDatabaseConnectorMock(
                sessionCallback: () => wasCalled = true);
            var stateHandlerMock = GetStateHandlerMock();
            var crewCheckerMock = GetCrewCheckerMock();

            var settingsPath = Path.GetFullPath(SettingsPath);

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices((_, services) => ConfigureServices(services, databaseConnectorMock, stateHandlerMock, crewCheckerMock))
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
            Mock<IStateHandler> stateHandlerMock, Mock<ICrewChecker> crewCheckerMock)
        {
            services.AddHostedService<Workers.Crew.Worker>();

            services.AddTransient<Mock<ILogger>>();
            services.AddSingleton(databaseConnectorMock.Object);
            services.AddSingleton(stateHandlerMock.Object);

            services.AddSingleton(crewCheckerMock.Object);
        }

        private static Mock<ICrewChecker> GetCrewCheckerMock(Action getCrewingElements = default)
        {
            var crewingElementsMock = new List<CrewingElement> { Mock.Of<CrewingElement>() };

            var result = new Mock<ICrewChecker>();

            result
                .Setup(m => m.GetCrewingElementsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .Callback(() => getCrewingElements?.Invoke())
                .Returns(Task.FromResult((IEnumerable<CrewingElement>)crewingElementsMock));

            return result;
        }

        #endregion Private Methods
    }
}