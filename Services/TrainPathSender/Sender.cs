using Common.Interfaces;
using Common.Models;
using CredentialChannelFactory;
using EnumerableExtensions;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrainPathImportService;
using TrainPathSender.Converters;
using TrainPathSender.Extensions;

namespace TrainPathSender
{
    public class Sender
        : ITrainPathSender
    {
        #region Private Fields

        private readonly ConcurrentQueue<IEnumerable<TrainRun>> importsQueue = new ConcurrentQueue<IEnumerable<TrainRun>>();
        private readonly ILogger<Sender> logger;

        private Factory<TrainPathImportWebFacadeChannel> channelFactory;
        private TrainRun2ImportPaths converter;
        private IEnumerable<string> ignoreTrainTypes;
        private bool logRequests;
        private AsyncRetryPolicy retryPolicy;
        private Task senderTask;

        #endregion Private Fields

        #region Public Constructors

        public Sender(ILogger<Sender> logger)
        {
            this.logger = logger;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Add(IEnumerable<TrainRun> trainRuns)
        {
            var filtereds = trainRuns
                .Where(r => r != default
                    && (!ignoreTrainTypes.AnyItem() || !ignoreTrainTypes.Contains(r.Zuggattung))).ToArray();

            if (filtereds.AnyItem())
            {
                importsQueue.Enqueue(filtereds);
            }
        }

        public Task ExecuteAsync(DateTime ivuDatum, string sessionKey, CancellationToken cancellationToken)
        {
            if (senderTask == default)
            {
                converter.Initialize(
                    ivuDatum: ivuDatum,
                    sessionKey: sessionKey);

                cancellationToken.Register(() => StopTask());

                senderTask = retryPolicy?.ExecuteAsync(
                    action: (token) => RunSenderAsync(token),
                    cancellationToken: cancellationToken);
            }

            return senderTask;
        }

        public void Initialize(string host, int port, string path, string username, string password, bool isHttps,
            int retryTime, string infrastructureManager, string orderingTransportationCompany, string stoppingReasonStop,
            string stoppingReasonPass, string trainPathStateRun, string trainPathStateCancelled, string importProfile,
            IEnumerable<string> ignoreTrainTypes, IEnumerable<string> locationShortnames, bool logRequests)
        {
            this.ignoreTrainTypes = ignoreTrainTypes;
            this.logRequests = logRequests;

            converter = new TrainRun2ImportPaths(
                infrastructureManager: infrastructureManager,
                orderingTransportationCompany: orderingTransportationCompany,
                stoppingReasonStop: stoppingReasonStop,
                stoppingReasonPass: stoppingReasonPass,
                trainPathStateRun: trainPathStateRun,
                trainPathStateCancelled: trainPathStateCancelled,
                importProfile: importProfile,
                locationShortnames: locationShortnames);

            channelFactory = new Factory<TrainPathImportWebFacadeChannel>(
                host: host,
                port: port,
                path: path,
                userName: username,
                password: password,
                isHttps: isHttps,
                notIgnoreCertificateErrors: true);

            logger.LogDebug(
                "Zugtrassen werden gesendet an {host}:{port}/{path}.",
                host,
                port,
                path);

            retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    sleepDurationProvider: _ => TimeSpan.FromSeconds(retryTime),
                    onRetry: (exception, reconnection) => OnRetry(
                        exception: exception,
                        reconnection: reconnection));
        }

        #endregion Public Methods

        #region Private Methods

        private void OnRetry(Exception exception, TimeSpan reconnection)
        {
            while (exception.InnerException != default) exception = exception.InnerException;

            logger.LogError(
                exception,
                "Fehler beim Senden der Trassendaten an IVU.rail: {message}\r\n" +
                "Die Verbindung wird in {reconection} Sekunden wieder versucht.",
                exception.Message,
                reconnection.TotalSeconds);
        }

        private async Task RunSenderAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!importsQueue.IsEmpty)
                {
                    importsQueue.TryPeek(out IEnumerable<TrainRun> currentImport);

                    var currentPaths = converter.Convert(currentImport);

                    if (currentPaths != default)
                    {
                        if (logRequests)
                        {
                            var trainPaths = currentPaths.trainPathImportRequest.trainPaths.Serialize();

                            logger.LogDebug(
                                "Es werden die folgenden Trassen an IVU.rail gesendet: {trainPaths}",
                                trainPaths);
                        }
                        else
                        {
                            logger.LogDebug(
                                "Es werden {Trassenzahl} Trassen an IVU.rail gesendet.",
                                currentPaths.trainPathImportRequest.trainPaths.Length);
                        }

                        using var channel = channelFactory.Get();
                        var response = await channel.importTrainPathsAsync(currentPaths);

                        if (response.trainPathImportResponse != default)
                        {
                            logger.LogDebug(
                                "Trassen wurden mit folgender ID an IVU.rail gesendet: {id}.",
                                response.trainPathImportResponse.protocolTransactionId);
                        }
                    }

                    importsQueue.TryDequeue(out _);
                }
            }
        }

        private void StopTask()
        {
            senderTask = default;

            importsQueue.Clear();
        }

        #endregion Private Methods
    }
}