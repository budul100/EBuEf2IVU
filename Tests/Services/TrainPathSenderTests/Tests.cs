using Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;
using TrainPathSender;

namespace TrainPathSenderTests
{
    // Database must be activated for tests!

    public class Tests
    {
        #region Private Fields

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

            Assert.True(messages.Length > 0);
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

            loggerFactory = new NullLoggerFactory();
            var logger = loggerFactory.CreateLogger<Sender>();

            var senderSettings = config
                .GetSection(nameof(Common.Settings.TrainPathSender))
                .Get<Common.Settings.TrainPathSender>();

            var ignoreTrainTypes = senderSettings.IgnoreTrainTypes?.Split(
                separator: Common.Settings.TrainPathSender.SettingsSeparator,
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
                infrastructureManager: senderSettings.InfrastructureManager,
                orderingTransportationCompany: senderSettings.OrderingTransportationCompany,
                stoppingReasonStop: senderSettings.StoppingReasonStop,
                stoppingReasonPass: senderSettings.StoppingReasonPass,
                trainPathStateRun: senderSettings.TrainPathStateRun,
                trainPathStateCancelled: senderSettings.TrainPathStateCancelled,
                importProfile: senderSettings.ImportProfile,
                ignoreTrainTypes: ignoreTrainTypes,
                locationShortnames: default);
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

            var cancellationTokenSource = new CancellationTokenSource();

            sender.ExecuteAsync(
                ivuDatum: query.Result.IVUDatum,
                sessionKey: query.Result.SessionKey,
                cancellationToken: cancellationTokenSource.Token);
        }

        #endregion Public Methods
    }
}