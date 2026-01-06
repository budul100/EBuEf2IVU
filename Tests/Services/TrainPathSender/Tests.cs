using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using EBuEf2IVU.Services.TrainPathSender;
using EBuEf2IVU.Shareds.Commons.Extensions;
using EBuEf2IVU.Shareds.Commons.Models;
using EBuEf2IVU.Shareds.Commons.Settings;
using EBuEf2IVU.Shareds.TestsBase;
using Newtonsoft.Json;
using System.ServiceModel;

namespace EBuEf2IVU.Shareds.TrainPathSenderTests
{
    // Database must be activated for tests!

    public class Tests
        : Base, IDisposable
    {
        #region Private Fields

        private const string MessagesPath = @"..\..\..\Messages.json";
        private const string SettingsPath = @"..\..\..\Tests.xml";

        private readonly IConfigurationRoot config;
        private readonly NullLoggerFactory loggerFactory;
        private readonly Sender sender;

        #endregion Private Fields

        #region Public Constructors

        public Tests()
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
                .GetSection(nameof(TrainPathSender))
                .Get<TrainPathSender>();

            var ignoreTrainTypes = senderSettings.IgnoreTrainTypes?.Split(
                separator: TrainPathSender.SettingsSeparator,
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
                trainPathStateAltered: senderSettings.TrainPathStateAltered,
                trainPathStateCancelled: senderSettings.TrainPathStateCancelled,
                importProfile: senderSettings.ImportProfile,
                ignoreTrainTypes: ignoreTrainTypes,
                locationShortnames: default,
                logRequests: senderSettings.LogRequests);
        }

        #endregion Public Constructors

        #region Public Methods

        [Fact]
        public void ConvertTrainPathMessages()
        {
            var content = File.ReadAllText(MessagesPath);
            var messages = JsonConvert.DeserializeObject<TrainPathMessage[]>(content);

            Assert.NotEmpty(messages);
        }

        public void Dispose()
        {
            loggerFactory?.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task InitialImport()
        {
            var connectorSettings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            var databaseConnector = new Services.DatabaseConnector.Connector(loggerFactory.CreateLogger<Services.DatabaseConnector.Connector>());
            var connectionString = connectorSettings.GetDBConnectionString();

            databaseConnector.Initialize(
                connectionString: connectionString,
                retryTime: connectorSettings.RetryTime,
                cancellationToken: new CancellationToken());

            var query = await databaseConnector.GetEBuEfSessionAsync();

            var trainRuns = await databaseConnector.GetTrainRunsPlanAsync(
                timetableId: query.FahrplanId,
                weekday: query.Wochentag,
                preferPrognosis: false);

            using var cancellationTokenSource = new CancellationTokenSource();

            sender.Add(trainRuns);

            var task = Task.Run(() => sender.ExecuteAsync(
                ivuDatum: query.IVUDatum,
                sessionKey: query.SessionKey,
                cancellationToken: cancellationTokenSource.Token));

            var hasCommunicationException = false;

            try
            {
                await task.WaitAsync(
                    TimeSpan.FromSeconds(5), 
                    CancellationToken.None);
            }
            catch (CommunicationException)
            {
                hasCommunicationException = true;
            }
            finally
            {
                cancellationTokenSource.Cancel();
            }

            Assert.True(hasCommunicationException);
        }

        #endregion Public Methods
    }
}