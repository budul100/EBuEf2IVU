using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Commons.Enums;
using Commons.EventsArgs;
using Commons.Extensions;
using Commons.Interfaces;
using Commons.Models;
using EBuEf2IVUTestBase;
using Moq;
using NUnit.Framework;

namespace EBuEf2IVUCrewTests
{
    public class Tests
        : TestsBase
    {
        #region Public Methods

        [Test]
        public void CheckCrewsOnRunningSessions()
        {
            var wasCalled = false;

            var databaseConnectorMock = GetDatabaseConnectorMock(
                sendTrainRuns: true);
            var stateHandlerMock = GetStateHandlerMock(
                sessionStatusType: StateType.IsRunning);
            var crewCheckerMock = GetCrewCheckerMock(
                getCrewingElements: () => wasCalled = true);

            var settingsPath = Path.GetFullPath(@"..\..\..\..\..\..\Programs\EBuEf2IVUCrew\ebuef2ivucrew-settings.example.xml");

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices(services => ConfigureServices(services, databaseConnectorMock, stateHandlerMock, crewCheckerMock))
                .Build();

            var cancellationTokenSource = new CancellationTokenSource();
            host.StartAsync(cancellationTokenSource.Token);

            Assert.That(wasCalled, Is.True);
        }

        [Test]
        public void InitializeSessionAfterRestart()
        {
            var wasCalled = false;

            var cancellationTokenSource = new CancellationTokenSource();

            var databaseConnectorMock = GetDatabaseConnectorMock(
                sessionCallback: () => wasCalled = true);
            var stateHandlerMock = GetStateHandlerMock();
            var crewCheckerMock = GetCrewCheckerMock();

            var settingsPath = Path.GetFullPath(@"..\..\..\..\..\..\Programs\EBuEf2IVUCrew\ebuef2ivucrew-settings.example.xml");

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices((_, services) => ConfigureServices(services, databaseConnectorMock, stateHandlerMock, crewCheckerMock))
                .Build();

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

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureServices(IServiceCollection services, Mock<IDatabaseConnector> databaseConnectorMock,
            Mock<IStateHandler> stateHandlerMock, Mock<ICrewChecker> crewCheckerMock)
        {
            services.AddHostedService<EBuEf2IVUCrew.Worker>();

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