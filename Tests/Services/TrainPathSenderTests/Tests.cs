using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Commons.Extensions;
using Commons.Models;
using Commons.Settings;
using Newtonsoft.Json;
using TrainPathSender;

namespace TrainPathSenderTests
{
    // Database must be activated for tests!

    public class Tests
    {
        #region Private Fields

        private const string SettingsPath = @"..\..\..\TrainPathSenderTests.example.xml";

        private IConfigurationRoot config;
        private NullLoggerFactory loggerFactory;
        private Sender sender;

        #endregion Private Fields

        #region Public Methods

        [Test]
        public void ConvertTrainPathMessages()
        {
            var content = File.ReadAllText("TrainPathMessages.json");
            var messages = JsonConvert.DeserializeObject<TrainPathMessage[]>(content);

            Assert.That(messages.Length > 0, Is.True);
        }

        [SetUp]
        public void Init()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddXmlFile(
                path: Path.GetFullPath(SettingsPath),
                optional: false,
                reloadOnChange: false);

            config = configBuilder.Build();

            loggerFactory = new NullLoggerFactory();
            var logger = loggerFactory.CreateLogger<Sender>();

            var senderSettings = config
                .GetSection(nameof(Commons.Settings.TrainPathSender))
                .Get<Commons.Settings.TrainPathSender>();

            var ignoreTrainTypes = senderSettings.IgnoreTrainTypes?.Split(
                separator: Commons.Settings.TrainPathSender.SettingsSeparator,
                options: StringSplitOptions.RemoveEmptyEntries);

            sender = new Sender(logger);

            sender.Initialize(
                host: senderSettings.Host,
                port: senderSettings.Port ?? 0,
                isHttps: senderSettings.IsHttps ?? false,
                username: senderSettings.Username,
                password: senderSettings.Password,
                path: senderSettings.Path,
                retryTime: senderSettings.RetryTime,
                timeoutInSecs: default,
                infrastructureManager: senderSettings.InfrastructureManager,
                orderingTransportationCompany: senderSettings.OrderingTransportationCompany,
                stoppingReasonStop: senderSettings.StoppingReasonStop,
                stoppingReasonPass: senderSettings.StoppingReasonPass,
                trainPathStateRun: senderSettings.TrainPathStateRun,
                trainPathStateCancelled: senderSettings.TrainPathStateCancelled,
                importProfile: senderSettings.ImportProfile,
                ignoreTrainTypes: ignoreTrainTypes,
                locationShortnames: default,
                logRequests: senderSettings.LogRequests);
        }

        [Test]
        public void InitialImport()
        {
            var connectorSettings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            var databaseConnector = new DatabaseConnector.Connector(loggerFactory.CreateLogger<DatabaseConnector.Connector>());
            var connectionString = connectorSettings.GetDBConnectionString();

            databaseConnector.Initialize(
                connectionString: connectionString,
                retryTime: connectorSettings.RetryTime,
                cancellationToken: new CancellationToken());

            var query = databaseConnector.GetEBuEfSessionAsync();
            query.Wait();

            var trainRuns = databaseConnector.GetTrainRunsPlanAsync(
                timetableId: query.Result.FahrplanId,
                weekday: query.Result.Wochentag,
                preferPrognosis: false).Result;

            var cancellationTokenSource = new CancellationTokenSource();

            sender.Add(trainRuns);

            sender.ExecuteAsync(
                ivuDatum: query.Result.IVUDatum,
                sessionKey: query.Result.SessionKey,
                cancellationToken: cancellationTokenSource.Token);
        }

        #endregion Public Methods
    }
}