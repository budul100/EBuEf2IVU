using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Commons.Extensions;
using Commons.Interfaces;
using CrewChecker;
using EBuEf2IVUTestBase;
using Moq;

namespace CrewCheckerTests
{
    public class Tests
        : TestsBase
    {
        #region Private Fields

        // Set this value as true if you have an active IVU instance running.
        private const bool IVUIsRunning = true;

        private const string SettingsPath = @"..\..\..\CrewCheckerTests.example.xml";

        private CancellationTokenSource cancellationTokenSource;
        private IConfigurationRoot config;

        #endregion Private Fields

        #region Public Methods

        [TearDown]
        public void Cleanup()
        {
            cancellationTokenSource?.Dispose();
        }

        [Test]
        public void CreateCrewingElements()
        {
            var hasException = false;

            var loggerMock = GetLoggerMock<Checker>(() => hasException = true);

            var settingsPath = Path.GetFullPath(SettingsPath);

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices(services => ConfigureServices(services, loggerMock))
                .Build();

            var checker = host.Services.GetService<ICrewChecker>();

            var settings = config
                .GetSection(nameof(Commons.Settings.CrewChecker))
                .Get<Commons.Settings.CrewChecker>();

            var serviceInterval = new TimeSpan(
                hours: 0,
                minutes: 0,
                seconds: settings.AbfrageIntervalSek);

            var queryDurationPast = new TimeSpan(
                hours: 0,
                minutes: settings.AbfrageVergangenheitMin * -1,
                seconds: 0);

            var queryDurationFuture = new TimeSpan(
                hours: 0,
                minutes: settings.AbfrageZukunftMin,
                seconds: 0);

            var appServerHost = settings.GetIVUAppServerHost();
            var appServerPort = settings.GetIVUAppServerPort();
            var isHttps = settings.IsIVUAppServerHttps();

            checker.Initialize(
                host: appServerHost,
                port: appServerPort,
                isHttps: isHttps,
                username: settings.Username,
                password: settings.Password,
                path: settings.Path,
                division: settings.Division,
                planningLevel: settings.PlanningLevel,
                retryTime: settings.RetryTime);

            Task.Run(() => checker.GetCrewingElementsAsync(
                tripNumbers: ["15521", "ABC", "", default, "15521"],
                date: new DateTime(2024, 01, 11),
                cancellationToken: cancellationTokenSource.Token));

            Thread.Sleep(2000);

            Assert.That(hasException, IVUIsRunning ? Is.False : Is.True);
        }

        [SetUp]
        public void Init()
        {
            var path = Path.GetFullPath(SettingsPath);

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddXmlFile(
                path: path,
                optional: false,
                reloadOnChange: false);

            config = configBuilder.Build();
            cancellationTokenSource = new CancellationTokenSource();
        }

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureServices(IServiceCollection services, Mock<ILogger<Checker>> loggerMock)
        {
            services.AddSingleton(loggerMock.Object);
            services.AddSingleton<ICrewChecker, Checker>();
        }

        #endregion Private Methods
    }
}