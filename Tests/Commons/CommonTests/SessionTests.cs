using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Commons.EventsArgs;
using Commons.Extensions;
using Commons.Interfaces;
using Commons.Settings;
using Moq;
using NUnit.Framework;

namespace CommonTests
{
    // Database must be activated for tests!
    // Check hosts file on C:\Windows\System32\drivers\etc for the following entry:
    // 127.0.0.1 db.ebuef
    // Check that the DB user ebuef2ivucore with rights for SELECT, INSERT, UPDATE is created

    public class SessionTests
    {
        #region Private Fields

        private CancellationTokenSource cancellationTokenSource;
        private IConfigurationRoot config;

        #endregion Private Fields

        #region Public Methods

        [Test]
        public void GetSessionData()
        {
            var loggerMock = new Mock<ILogger<DatabaseConnector.Connector>>();

            var databaseConnector = new DatabaseConnector.Connector(loggerMock.Object);

            var connectorSettings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            var connectionString = connectorSettings.GetDBConnectionString();

            databaseConnector.Initialize(
                connectionString: connectionString,
                retryTime: connectorSettings.RetryTime,
                cancellationToken: new CancellationToken());

            var query = databaseConnector.GetEBuEfSessionAsync();
            query.Wait();

            var curOffset = TimeZoneInfo.Local.BaseUtcOffset;
            var expOffset = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time").BaseUtcOffset;

            // The values must possibly adjusted if a new database was imported

            Assert.That(query.Result.IVUDatum, Is.EqualTo(query.Result.IVUDatum.Date));
            Assert.That(query.Result.Wochentag, Is.EqualTo(DayOfWeek.Friday));
            Assert.That(query.Result.SessionStart, Is.EqualTo(new TimeSpan(10, 0, 0).Add(curOffset).Subtract(expOffset)));
        }

        [Test]
        public void GetSessionNotStarted()
        {
            var connectorSettings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            var multicastReceiverMock = new Mock<IMulticastReceiver>();
            var mqttReceiverMock = new Mock<IMQTTReceiver>();
            var loggerMock = new Mock<ILogger<StateHandler.Handler>>();

            var databaseConnectorMock = new Mock<IDatabaseConnector>();
            databaseConnectorMock.Setup(c => c.GetEBuEfSessionActiveAsync()).Returns(Task.FromResult(false));

            var sessionStateHandler = new StateHandler.Handler(
                logger: loggerMock.Object,
                databaseConnector: databaseConnectorMock.Object,
                multicastReceiver: multicastReceiverMock.Object,
                mqttReceiver: mqttReceiverMock.Object);

            var wasCalled = false;
            sessionStateHandler.SessionChangedEvent += (o, e) => wasCalled = e.StateType == Commons.Enums.StateType.IsRunning;

            Task.WaitAny(sessionStateHandler.ExecuteAsync(cancellationTokenSource.Token));

            Assert.That(wasCalled, Is.False);
        }

        [Test]
        public void GetSessionPreparationTwice()
        {
            var connectorSettings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            var multicastReceiverMock = new Mock<IMulticastReceiver>();
            var mqttReceiverMock = new Mock<IMQTTReceiver>();
            var loggerMock = new Mock<ILogger<StateHandler.Handler>>();

            var databaseConnectorMock = new Mock<IDatabaseConnector>();

            var sessionStateHandler = new StateHandler.Handler(
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

            Assert.That(eventsSend, Is.EqualTo(2));
        }

        [Test]
        public void GetSessionStarted()
        {
            var connectorSettings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            var multicastReceiverMock = new Mock<IMulticastReceiver>();
            var mqttReceiverMock = new Mock<IMQTTReceiver>();
            var loggerMock = new Mock<ILogger<StateHandler.Handler>>();

            var databaseConnectorMock = new Mock<IDatabaseConnector>();
            databaseConnectorMock
                .Setup(c => c.GetEBuEfSessionAsync())
                .Returns(Task.FromResult(new Commons.Models.EBuEfSession { Status = Commons.Enums.StateType.IsRunning }));

            var sessionStateHandler = new StateHandler.Handler(
                logger: loggerMock.Object,
                databaseConnector: databaseConnectorMock.Object,
                multicastReceiver: multicastReceiverMock.Object,
                mqttReceiver: mqttReceiverMock.Object);

            var wasCalled = false;
            sessionStateHandler.SessionChangedEvent += (o, e) => wasCalled = e.StateType == Commons.Enums.StateType.IsRunning;

            Task.WhenAny(sessionStateHandler.ExecuteAsync(cancellationTokenSource.Token));

            Assert.That(wasCalled, Is.True);
        }

        [SetUp]
        public void Init()
        {
            var path = Path.GetFullPath(@"..\..\..\..\..\..\Programs\EBuEf2IVUPath\ebuef2ivupath-settings.example.xml");

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddXmlFile(
                path: path,
                optional: false,
                reloadOnChange: false);

            config = configBuilder.Build();
            cancellationTokenSource = new CancellationTokenSource();
        }

        #endregion Public Methods
    }
}