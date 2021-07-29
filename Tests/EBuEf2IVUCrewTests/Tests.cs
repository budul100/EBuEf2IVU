using Common.Extensions;
using Common.Interfaces;
using Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EBuEf2IVUCrewTests
{
    public class Tests
    {
        #region Public Methods

        [Test]
        public void CheckCrewsOnRunningSessions()
        {
            var wasCalled = false;

            var cancellationTokenSource = new CancellationTokenSource();

            var crewingElementsMock = new List<CrewingElement> { Mock.Of<CrewingElement>() };

            var crewCheckerMock = new Mock<ICrewChecker>();
            crewCheckerMock
                .Setup(m => m.GetCrewingElementsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .Callback(() => { wasCalled = true; cancellationTokenSource.Cancel(); })
                .Returns(Task.FromResult((IEnumerable<CrewingElement>)crewingElementsMock));

            var settingsPath = Path.GetFullPath(@"..\..\..\..\..\Programs\EBuEf2IVUCrew\ebuef2ivucrew-settings.example.xml");

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices((hostContext, services) => ConfigureServices(services, crewCheckerMock.Object));

            host.Build().StartAsync(cancellationTokenSource.Token);

            Assert.True(wasCalled);
        }

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureServices(IServiceCollection services, ICrewChecker crewChecker)
        {
            services.AddHostedService<EBuEf2IVUCrew.Worker>();

            services.AddTransient<Mock<ILogger>>();

            var messageReceiverMock = new Mock<IMessageReceiver>();
            services.AddSingleton(messageReceiverMock.Object);

            var session = new EBuEfSession
            {
                IVUDatum = DateTime.Today,
                SessionStart = DateTime.Now,
            };

            var trainRunsMock = new List<TrainRun> { Mock.Of<TrainRun>() };

            var databaseConnectorMock = new Mock<IDatabaseConnector>();
            databaseConnectorMock.Setup(m => m.GetEBuEfSessionActiveAsync()).Returns(Task.FromResult(true));
            databaseConnectorMock.Setup(m => m.GetEBuEfSessionAsync()).Returns(Task.FromResult(session));
            databaseConnectorMock.Setup(m => m.GetTrainRunsDispoAsync(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>())).Returns(Task.FromResult((IEnumerable<TrainRun>)trainRunsMock));

            services.AddSingleton(databaseConnectorMock.Object);

            services.AddSingleton<IStateHandler, StateHandler.Handler>();

            services.AddSingleton(crewChecker);
        }

        #endregion Private Methods
    }
}