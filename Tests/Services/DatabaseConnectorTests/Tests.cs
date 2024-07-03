using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Commons.Extensions;
using Commons.Interfaces;
using Commons.Models;
using Commons.Settings;
using DatabaseConnector;
using EBuEf2IVUTestBase;
using Moq;

namespace DatabaseConnectorTests
{
    public class Tests
        : TestsBase
    {
        #region Private Fields

        private const string SettingsPath = @"..\..\..\DatabaseConnectorTests.example.xml";
        private CancellationTokenSource cancellationTokenSource;
        private IConfigurationRoot config;

        #endregion Private Fields

        #region Public Methods

        [Test]
        public void CreateCrewingElements()
        {
            var hasException = false;

            var loggerMock = GetLoggerMock<Connector>(() => hasException = true);

            var settingsPath = Path.GetFullPath(SettingsPath);

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices(services => ConfigureServices(services, loggerMock))
                .Build();

            var connector = host.Services.GetService<IDatabaseConnector>();

            var connectorSettings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            var connectionString = connectorSettings.GetDBConnectionString();

            connector.Initialize(
                connectionString: connectionString,
                retryTime: connectorSettings.RetryTime,
                cancellationToken: new CancellationToken());

            var crewingElement = new CrewingElement
            {
                BetriebsstelleVon = "XBG",
                BetriebsstelleNach = "XWF",
                DienstKurzname = "Test",
                PersonalNachname = "Test",
                PersonalNummer = "XYZ1000",
                Zugnummer = "103",
                ZugnummerVorgaenger = "101"
            };

            Task.Run(() => connector.SetCrewingsAsync(new CrewingElement[] { crewingElement })).Wait();

            Assert.That(hasException, Is.False);
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

        private static void ConfigureServices(IServiceCollection services, Mock<ILogger<Connector>> loggerMock)
        {
            services.AddSingleton(loggerMock.Object);
            services.AddSingleton<IDatabaseConnector, Connector>();
        }

        #endregion Private Methods
    }
}