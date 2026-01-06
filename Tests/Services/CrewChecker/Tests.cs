using System;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EBuEf2IVU.Services.CrewChecker;
using EBuEf2IVU.Shareds.Commons.Extensions;
using EBuEf2IVU.Shareds.Commons.Interfaces;
using EBuEf2IVU.Shareds.TestsBase;
using Moq;

namespace EBuEf2IVU.Shareds.CrewCheckerTests
{
    public class Tests
        : Base, IDisposable
    {
        #region Private Fields

        private const string SettingsPath = @"..\..\..\Tests.xml";

        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly IConfigurationRoot config;

        #endregion Private Fields

        #region Public Constructors

        public Tests()
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

        #endregion Public Constructors

        #region Public Methods

        [Fact]
        public async Task CreateCrewingElements()
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

            var task = checker.GetCrewingElementsAsync(
                tripNumbers: ["15521", "ABC", "", default, "15521"],
                date: new DateTime(2024, 01, 11),
                cancellationToken: cancellationTokenSource.Token);

            var hasCommunicationException = false;

            try
            {
                await task;
            }
            catch (CommunicationException)
            {
                hasCommunicationException = true;
            }

            Assert.True(hasCommunicationException || hasException);
        }

        public void Dispose()
        {
            cancellationTokenSource?.Dispose();
            GC.SuppressFinalize(this);
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