using Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
            var connectorSettings = config
                .GetSection(nameof(EBuEf2IVUBase.Settings.EBuEfDBConnector))
                .Get<EBuEf2IVUBase.Settings.EBuEfDBConnector>();

            var loggerMock = new Mock<ILogger<DatabaseConnector.Connector>>();

            var databaseConnector = new DatabaseConnector.Connector(loggerMock.Object);

            databaseConnector.Initialize(
                connectionString: connectorSettings.ConnectionString,
                retryTime: connectorSettings.RetryTime,
                cancellationToken: new CancellationToken());

            var query = databaseConnector.GetEBuEfSessionAsync();
            query.Wait();

            Assert.That(query.Result.IVUDatum == query.Result.IVUDatum.Date, Is.True);
            Assert.That(query.Result.SessionStart.TimeOfDay == new TimeSpan(10, 0, 0), Is.True);
            Assert.That(query.Result.Wochentag == DayOfWeek.Friday, Is.True);
        }

        [Test]
        public void GetSessionNotStarted()
        {
            var connectorSettings = config
                .GetSection(nameof(EBuEf2IVUBase.Settings.EBuEfDBConnector))
                .Get<EBuEf2IVUBase.Settings.EBuEfDBConnector>();

            var messageReceiverMock = new Mock<IMessageReceiver>();
            var loggerMock = new Mock<ILogger<StateHandler.Handler>>();

            var databaseConnectorMock = new Mock<IDatabaseConnector>();
            databaseConnectorMock.Setup(c => c.GetEBuEfSessionActiveAsync()).Returns(Task.FromResult(false));

            var sessionStateHandler = new StateHandler.Handler(
                logger: loggerMock.Object,
                databaseConnector: databaseConnectorMock.Object,
                stateReceiver: messageReceiverMock.Object);

            var wasCalled = false;
            sessionStateHandler.SessionChangedEvent += (o, e) => wasCalled = e.StateType == Common.Enums.StateType.IsRunning;

            Task.WaitAny(sessionStateHandler.ExecuteAsync(cancellationTokenSource.Token));

            Assert.False(wasCalled);
        }

        [Test]
        public void GetSessionStartedAsync()
        {
            var connectorSettings = config
                .GetSection(nameof(EBuEf2IVUBase.Settings.EBuEfDBConnector))
                .Get<EBuEf2IVUBase.Settings.EBuEfDBConnector>();

            var messageReceiverMock = new Mock<IMessageReceiver>();
            var loggerMock = new Mock<ILogger<StateHandler.Handler>>();

            var databaseConnectorMock = new Mock<IDatabaseConnector>();
            databaseConnectorMock
                .Setup(c => c.GetEBuEfSessionAsync())
                .Returns(Task.FromResult(new Common.Models.EBuEfSession { Status = Common.Enums.StateType.IsRunning }));

            var sessionStateHandler = new StateHandler.Handler(
                logger: loggerMock.Object,
                databaseConnector: databaseConnectorMock.Object,
                stateReceiver: messageReceiverMock.Object);

            var wasCalled = false;
            sessionStateHandler.SessionChangedEvent += (o, e) => wasCalled = e.StateType == Common.Enums.StateType.IsRunning;

            Task.WhenAny(sessionStateHandler.ExecuteAsync(cancellationTokenSource.Token));

            Assert.True(wasCalled);
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