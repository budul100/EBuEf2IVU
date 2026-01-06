using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using EBuEf2IVU.Shareds.Commons.EventsArgs;
using EBuEf2IVU.Shareds.Commons.Extensions;
using EBuEf2IVU.Shareds.Commons.Interfaces;
using EBuEf2IVU.Shareds.Commons.Settings;
using Moq;

namespace EBuEf2IVU.Shareds.CommonsTests
{
    // Database must be activated for tests!
    // Check hosts file on C:\Windows\System32\drivers\etc for the following entry:
    // 127.0.0.1 db.ebuef
    // Check that the DB user ebuef2ivucore with rights for SELECT, INSERT, UPDATE is created

    public class SessionTests
        : IDisposable
    {
        #region Private Fields

        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly IConfigurationRoot config;

        #endregion Private Fields

        #region Public Constructors

        public SessionTests()
        {
            var path = Path.GetFullPath(@"..\..\..\..\..\..\Shareds\Host\Settings\settings.path.local.xml");

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

        public void Dispose()
        {
            cancellationTokenSource?.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task GetSessionData()
        {
            var loggerMock = new Mock<ILogger<Services.DatabaseConnector.Connector>>();

            var databaseConnector = new Services.DatabaseConnector.Connector(loggerMock.Object);

            var connectorSettings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            var connectionString = connectorSettings.GetDBConnectionString();

            databaseConnector.Initialize(
                connectionString: connectionString,
                retryTime: connectorSettings.RetryTime,
                cancellationToken: new CancellationToken());

            var query = await databaseConnector.GetEBuEfSessionAsync();

            var curOffset = TimeZoneInfo.Local.BaseUtcOffset;
            var expOffset = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time").BaseUtcOffset;

            // The values must possibly adjusted if a new database was imported

            Assert.Equal(query.IVUDatum.Date, query.IVUDatum);
            Assert.Equal(DayOfWeek.Friday, query.Wochentag);
            Assert.Equal(new TimeSpan(10, 0, 0).Add(curOffset).Subtract(expOffset), query.SessionStart);
        }

        [Fact]
        public async Task GetSessionNotStarted()
        {
            var connectorSettings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            var multicastReceiverMock = new Mock<IMulticastReceiver>();
            var mqttReceiverMock = new Mock<IMQTTReceiver>();
            var loggerMock = new Mock<ILogger<Services.StateHandler.Handler>>();

            var databaseConnectorMock = new Mock<IDatabaseConnector>();
            databaseConnectorMock.Setup(c => c.GetEBuEfSessionActiveAsync()).Returns(Task.FromResult(false));

            var sessionStateHandler = new Services.StateHandler.Handler(
                logger: loggerMock.Object,
                databaseConnector: databaseConnectorMock.Object,
                multicastReceiver: multicastReceiverMock.Object,
                mqttReceiver: mqttReceiverMock.Object);

            var wasCalled = false;
            sessionStateHandler.SessionChangedEvent += (o, e) => wasCalled = e.StateType == Commons.Enums.StateType.IsRunning;

            await sessionStateHandler.ExecuteAsync(cancellationTokenSource.Token);

            Assert.False(wasCalled);
        }

        [Fact]
        public void GetSessionPreparationTwice()
        {
            var connectorSettings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            var multicastReceiverMock = new Mock<IMulticastReceiver>();
            var mqttReceiverMock = new Mock<IMQTTReceiver>();
            var loggerMock = new Mock<ILogger<Services.StateHandler.Handler>>();

            var databaseConnectorMock = new Mock<IDatabaseConnector>();

            var sessionStateHandler = new Services.StateHandler.Handler(
                logger: loggerMock.Object,
                databaseConnector: databaseConnectorMock.Object,
                multicastReceiver: multicastReceiverMock.Object,
                mqttReceiver: mqttReceiverMock.Object);

            sessionStateHandler.Initialize(
                host: default,
                port: default,
                retryTime: 30,
                sessionStartPattern: "ZN SET DISPATCH",
                sessionStatusPattern: "SESSION NEW STATUS $");

            var eventsSend = 0;
            sessionStateHandler.SessionChangedEvent += (o, e) =>
            {
                if (e.StateType == Commons.Enums.StateType.InPreparation) eventsSend++;
            };

            Task.WhenAny(sessionStateHandler.ExecuteAsync(cancellationTokenSource.Token));

            multicastReceiverMock.Raise(r => r.MessageReceivedEvent += null, new MessageReceivedArgs("SESSION NEW STATUS 1"));
            multicastReceiverMock.Raise(r => r.MessageReceivedEvent += null, new MessageReceivedArgs("SESSION NEW STATUS 1"));

            Assert.Equal(2, eventsSend);
        }

        [Fact]
        public void GetSessionStarted()
        {
            var connectorSettings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            var multicastReceiverMock = new Mock<IMulticastReceiver>();
            var mqttReceiverMock = new Mock<IMQTTReceiver>();
            var loggerMock = new Mock<ILogger<Services.StateHandler.Handler>>();

            var databaseConnectorMock = new Mock<IDatabaseConnector>();
            databaseConnectorMock
                .Setup(c => c.GetEBuEfSessionAsync())
                .Returns(Task.FromResult(new Commons.Models.EBuEfSession { Status = Commons.Enums.StateType.IsRunning }));

            var sessionStateHandler = new Services.StateHandler.Handler(
                logger: loggerMock.Object,
                databaseConnector: databaseConnectorMock.Object,
                multicastReceiver: multicastReceiverMock.Object,
                mqttReceiver: mqttReceiverMock.Object);

            var wasCalled = false;
            sessionStateHandler.SessionChangedEvent += (o, e) => wasCalled = e.StateType == Commons.Enums.StateType.IsRunning;

            Task.WhenAny(sessionStateHandler.ExecuteAsync(cancellationTokenSource.Token));

            Assert.True(wasCalled);
        }

        #endregion Public Methods
    }
}