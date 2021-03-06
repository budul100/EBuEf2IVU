﻿using Common.Interfaces;
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

namespace TrainPathSender
{
    public class Sender
        : ITrainPathSender
    {
        #region Private Fields

        private readonly ConcurrentQueue<importTrainPaths> importsQueue = new ConcurrentQueue<importTrainPaths>();
        private readonly ILogger<Sender> logger;

        private Factory<TrainPathImportWebFacadeChannel> channelFactory;
        private IEnumerable<string> ignoreTrainTypes;
        private Message2TrainRun messageConverter;
        private AsyncRetryPolicy retryPolicy;
        private TrainRun2ImportPaths trainRunConverter;

        #endregion Private Fields

        #region Public Constructors

        public Sender(ILogger<Sender> logger)
        {
            this.logger = logger;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Add(IEnumerable<TrainPathMessage> messages)
        {
            if (messages.AnyItem())
            {
                var trainRuns = messageConverter
                    .Get(messages).ToArray();

                Add(trainRuns);
            }
        }

        public void Add(IEnumerable<TrainRun> trainRuns)
        {
            var filtereds = trainRuns
                .Where(r => !ignoreTrainTypes.AnyItem() || !ignoreTrainTypes.Contains(r.Zuggattung)).ToArray();

            if (filtereds.AnyItem())
            {
                var imports = trainRunConverter.Get(filtereds);

                if (imports != default)
                    importsQueue.Enqueue(imports);
            }
        }

        public void Initialize(string host, int port, string path, string username, string password, bool isHttps,
            int retryTime, string sessionKey, DateTime sessionDate, string infrastructureManager,
            string orderingTransportationCompany, string stoppingReasonStop, string stoppingReasonPass,
            string trainPathStateRun, string trainPathStateCancelled, string importProfile, bool preferPrognosis,
            IEnumerable<string> ignoreTrainTypes, IEnumerable<string> locationShortnames)
        {
            this.ignoreTrainTypes = ignoreTrainTypes;

            messageConverter = new Message2TrainRun(preferPrognosis);

            trainRunConverter = new TrainRun2ImportPaths(
                sessionDate: sessionDate,
                sessionKey: sessionKey,
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
                    sleepDurationProvider: (p) => TimeSpan.FromSeconds(retryTime),
                    onRetry: (exception, reconnection) => OnRetry(
                        exception: exception,
                        reconnection: reconnection));
        }

        public Task RunAsnc(CancellationToken cancellationToken)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (token) => RunSenderAsync(token),
                cancellationToken: cancellationToken);

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private void OnRetry(Exception exception, TimeSpan reconnection)
        {
            while (exception.InnerException != null) exception = exception.InnerException;

            logger.LogError(
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
                    importsQueue.TryDequeue(out importTrainPaths currentImport);

                    if (currentImport != default)
                    {
                        logger.LogDebug(
                            "Es werden {Trassenzahl} Trassen an IVU.rail gesendet.",
                            currentImport.trainPathImportRequest.trainPaths.Count());

                        using var channel = channelFactory.Get();
                        var response = await channel.importTrainPathsAsync(currentImport);

                        if (response.trainPathImportResponse != default)
                        {
                            logger.LogDebug(
                                "Trassen wurden mit folgender ID an IVU.rail gesendet: {id}.",
                                response.trainPathImportResponse.protocolTransactionId);
                        }
                    }
                }
            }
        }

        #endregion Private Methods
    }
}