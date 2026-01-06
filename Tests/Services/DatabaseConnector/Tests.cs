using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EBuEf2IVU.Services.DatabaseConnector;
using EBuEf2IVU.Shareds.Commons.Extensions;
using EBuEf2IVU.Shareds.Commons.Interfaces;
using EBuEf2IVU.Shareds.Commons.Models;
using EBuEf2IVU.Shareds.Commons.Settings;
using EBuEf2IVU.Shareds.TestsBase;
using Moq;

namespace EBuEf2IVU.Shareds.DatabaseConnectorTests
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

            await connector.SetCrewingsAsync([crewingElement]);

            Assert.False(hasException);
        }

        public void Dispose()
        {
            cancellationTokenSource?.Dispose();
            GC.SuppressFinalize(this);
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