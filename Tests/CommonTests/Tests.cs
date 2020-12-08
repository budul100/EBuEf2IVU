using Epoch.net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;

namespace CommonTests
{
    // Database must be activated for tests!
    public class Tests
    {
        #region Private Fields

        private IConfigurationRoot config;
        private NullLoggerFactory loggerFactory;

        #endregion Private Fields

        #region Public Methods

        [SetUp]
        public void _Init()
        {
            var path = Path.GetFullPath(@"..\..\..\..\..\Programs\EBuEf2IVUPath\ebuef2ivupath-settings.example.xml");

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddXmlFile(
                path: path,
                optional: false,
                reloadOnChange: false);

            config = configBuilder.Build();

            loggerFactory = new NullLoggerFactory();
        }

        [Test]
        public void GetSessionAsync()
        {
            var connectorSettings = config
                .GetSection(nameof(EBuEf2IVUBase.Settings.EBuEfDBConnector))
                .Get<EBuEf2IVUBase.Settings.EBuEfDBConnector>();

            var databaseConnector = new DatabaseConnector.Connector(loggerFactory.CreateLogger<DatabaseConnector.Connector>());

            databaseConnector.Initialize(
                connectionString: connectorSettings.ConnectionString,
                retryTime: connectorSettings.RetryTime,
                sessionCancellationToken: new CancellationToken());

            var result = databaseConnector.GetEBuEfSessionAsync();

            result.Wait();

            Assert.That(result.Result.IVUDatum == DateTime.Today, Is.True);
        }

        [Test]
        public void TestUnixTime()
        {
            var timestamp = 1608292800;

            Assert.IsTrue(timestamp.ToDateTime().ToLocalTime().TimeOfDay == new TimeSpan(13, 0, 0));
        }

        #endregion Public Methods
    }
}