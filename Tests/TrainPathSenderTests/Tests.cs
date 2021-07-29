using Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrainPathSender;

namespace TrainPathSenderTests
{
    // Database must be activated for tests!
    public class Tests
    {
        #region Private Fields

        private const string SessionKey = "TestSessionKey";

        private IConfigurationRoot config;
        private NullLoggerFactory loggerFactory;
        private Sender sender;

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
            var logger = loggerFactory.CreateLogger<Sender>();

            var senderSettings = config
                .GetSection(nameof(EBuEf2IVUPath.Settings.TrainPathSender))
                .Get<EBuEf2IVUPath.Settings.TrainPathSender>();

            var ignoreTrainTypes = senderSettings.IgnoreTrainTypes?.Split(
                separator: EBuEf2IVUPath.Settings.TrainPathSender.SettingsSeparator,
                options: StringSplitOptions.RemoveEmptyEntries);

            sender = new Sender(logger);

            sender.Initialize(
                host: senderSettings.Host,
                port: senderSettings.Port,
                path: senderSettings.Path,
                username: senderSettings.Username,
                password: senderSettings.Password,
                isHttps: senderSettings.IsHttps,
                retryTime: senderSettings.RetryTime,
                sessionKey: SessionKey,
                sessionDate: DateTime.Today,
                infrastructureManager: senderSettings.InfrastructureManager,
                orderingTransportationCompany: senderSettings.OrderingTransportationCompany,
                stoppingReasonStop: senderSettings.StoppingReasonStop,
                stoppingReasonPass: senderSettings.StoppingReasonPass,
                trainPathStateRun: senderSettings.TrainPathStateRun,
                trainPathStateCancelled: senderSettings.TrainPathStateCancelled,
                importProfile: senderSettings.ImportProfile,
                preferPrognosis: senderSettings.PreferPrognosis,
                ignoreTrainTypes: ignoreTrainTypes,
                locationShortnames: default);
        }

        [Test]
        public void ConvertTrainPathMessages()
        {
            var content = File.ReadAllText("TrainPathMessages.json");
            var messages = JsonConvert.DeserializeObject<TrainPathMessage[]>(content);

            Assert.True(messages.Any());
        }

        [Test]
        public void InitialImport()
        {
            var connectorSettings = config
                .GetSection(nameof(EBuEf2IVUBase.Settings.EBuEfDBConnector))
                .Get<EBuEf2IVUBase.Settings.EBuEfDBConnector>();

            var databaseConnector = new DatabaseConnector.Connector(loggerFactory.CreateLogger<DatabaseConnector.Connector>());

            databaseConnector.Initialize(
                connectionString: connectorSettings.ConnectionString,
                retryTime: connectorSettings.RetryTime,
                cancellationToken: new CancellationToken());

            var query = databaseConnector.GetEBuEfSessionAsync();
            query.Wait();

            var trainRuns = databaseConnector.GetTrainRunsPlanAsync(
                timetableId: query.Result.FahrplanId,
                weekday: query.Result.Wochentag,
                preferPrognosis: false).Result;

            sender.Add(trainRuns);

            Task.WhenAll(sender.RunAsnc(new CancellationToken()));

            // Activate only if a soap server is running to receive a result
            // Task.WhenAll(sender.RunAsnc(new CancellationToken()));
        }

        #endregion Public Methods
    }
}