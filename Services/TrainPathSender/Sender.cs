using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Commons.Interfaces;
using Commons.Models;
using CredentialChannelFactory;
using EnumerableExtensions;
using Polly;
using Polly.Retry;
using TrainPathImportService110;
using TrainPathSender.Converters;
using TrainPathSender.Extensions;

namespace TrainPathSender
{
    public class Sender
        : ITrainPathSender
    {
        #region Private Fields

        private readonly ConcurrentQueue<IEnumerable<TrainRun>> importsQueue = new();
        private readonly ILogger<Sender> logger;
        private Factory<TrainPathImportWebFacadeChannel> channelFactory;
        private TrainRun2ImportPaths converter;
        private IEnumerable<string> ignoreTrainTypes;
        private bool logRequests;
        private AsyncRetryPolicy retryPolicy;
        private Task senderTask;
        private int? timeoutInSecs;

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

                cancellationToken.Register(StopTask);

                senderTask = retryPolicy?.ExecuteAsync(
                    action: RunSenderAsync,
                    cancellationToken: cancellationToken);
            }

            return senderTask;
        }

        public void Initialize(string host, int port, bool isHttps, string username, string password, string path,
            int retryTime, int? timeoutInSecs, string infrastructureManager, string orderingTransportationCompany,
            string stoppingReasonStop, string stoppingReasonPass, string trainPathStateRun,
            string trainPathStateCancelled, string importProfile, IEnumerable<string> ignoreTrainTypes,
            IEnumerable<string> locationShortnames, bool logRequests)
        {
            this.ignoreTrainTypes = ignoreTrainTypes;
            this.logRequests = logRequests;
            this.timeoutInSecs = timeoutInSecs;

            converter = new TrainRun2ImportPaths(
                infrastructureManager: infrastructureManager,
                orderingTransportationCompany: orderingTransportationCompany,
                stoppingReasonStop: stoppingReasonStop,
                stoppingReasonPass: stoppingReasonPass,
                trainPathStateRun: trainPathStateRun.GetTrainPathState(),
                trainPathStateCancelled: trainPathStateCancelled.GetTrainPathState(),
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
                    onRetry: OnRetry);
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

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
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

                        if (timeoutInSecs.HasValue)
                        {
                            channel.OperationTimeout = new TimeSpan(
                                hours: 0,
                                minutes: 0,
                                seconds: timeoutInSecs.Value);
                        }

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