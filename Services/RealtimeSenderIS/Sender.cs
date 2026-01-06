using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using EBuEf2IVU.Services.RealtimeSenderIS.Converters;
using EBuEf2IVU.Shareds.Commons.Interfaces;
using EBuEf2IVU.Shareds.Commons.Models;
using EnumerableExtensions;
using Polly;
using Polly.Retry;
using RealtimeSenderIS;

namespace EBuEf2IVU.Services.RealtimeSenderIS
{
    public class Sender(ILogger<Sender> logger)
        : IRealtimeSenderIS
    {
        #region Private Fields

        private readonly ILogger logger = logger;
        private readonly ConcurrentQueue<RealTimeInfoTO> messagesQueue = new();

        private RealTimeInformationImportFacadeClient client;
        private Message2RealtimeInfo converter;
        private AsyncRetryPolicy retryPolicy;
        private Task senderTask;

        #endregion Private Fields

        #region Public Methods

        public void Add(IEnumerable<VehicleAllocation> trainAllocations)
        {
            foreach (var allocation in trainAllocations)
            {
                var message = converter.Convert(
                    allocation: allocation);

                if (message != default)
                {
                    messagesQueue.Enqueue(message);
                }
            }
        }

        public void Add(TrainLeg trainLeg)
        {
            if (trainLeg != default)
            {
                var message = converter.Convert(
                    leg: trainLeg);

                if (message != default)
                {
                    messagesQueue.Enqueue(message);
                }
            }
        }

        public Task ExecuteAsync(DateTime ivuDatum, TimeSpan sessionStart, CancellationToken cancellationToken)
        {
            if (senderTask == default)
            {
                converter.Initialize(
                    ivuDatum: ivuDatum,
                    sessionStart: sessionStart);

                cancellationToken.Register(StopTask);

                senderTask = retryPolicy?.ExecuteAsync(
                    action: RunSenderAsync,
                    cancellationToken: cancellationToken);
            }

            return senderTask;
        }

        public void Initialize(string endpoint, string division, int? retryTime)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentException(
                    $"\"{nameof(endpoint)}\" darf nicht NULL oder ein Leerraumzeichen sein.",
                    nameof(endpoint));
            }

            if (string.IsNullOrWhiteSpace(division))
            {
                throw new ArgumentException(
                    $"\"{nameof(division)}\" darf nicht NULL oder ein Leerraumzeichen sein.",
                    nameof(division));
            }

            converter = new Message2RealtimeInfo(
                logger: logger,
                division: division);

            client = new RealTimeInformationImportFacadeClient();
            client.Endpoint.Address = new EndpointAddress(endpoint);

            if (retryTime.HasValue)
            {
                retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryForeverAsync(
                        sleepDurationProvider: _ => TimeSpan.FromSeconds(retryTime.Value),
                        onRetry: OnRetry);
            }
            else
            {
                retryPolicy = Policy
                    .Handle<Exception>()
                    .RetryAsync(0);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void OnRetry(Exception exception, TimeSpan reconnection)
        {
            while (exception.InnerException != default) exception = exception.InnerException;

            logger.LogError(
                exception,
                "Fehler beim Senden der Ist-Zeiten an IVU.rail: {message}\r\n" +
                "Die Verbindung wird in {reconnection} Sekunden wieder versucht.",
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
                if (!messagesQueue.IsEmpty)
                {
                    messagesQueue.TryPeek(out var currentMessage);

                    if (currentMessage != default)
                    {
                        var importInfo = new RealTimeInfoTO[1] { currentMessage };

                        var response = await client.importRealTimeInfoAsync(importInfo);
                        var result = response.importRealTimeInfoResponse1?.ToArray();

                        if (result.AnyItem())
                        {
                            var relevantValiditation = result[0];
                            var relevantMessage = currentMessage;

                            if (relevantValiditation.code == 0)
                            {
                                logger.LogDebug(
                                    "Ist-Zeit-Nachricht wurde erfolgreich an IVU.rail gesendet " +
                                    "(Zug: {trainNumber}, Betriebsstelle: {location}, Decoder: {decoder}).",
                                    relevantMessage.tripNumber,
                                    relevantMessage.stopArea,
                                    relevantMessage.vehicles.FirstOrDefault()?.number);
                            }
                            else
                            {
                                logger.LogError(
                                    "Fehlermeldung zur Ist-Zeit-Nachricht von IVU.rail empfangen " +
                                    "(Zug: {trainNumber}, Betriebsstelle: {location}, Decoder: {decoder}): {message}.",
                                    relevantValiditation.message,
                                    relevantMessage.tripNumber,
                                    relevantMessage.stopArea,
                                    relevantMessage.vehicles.FirstOrDefault()?.number);
                            }
                        }
                    }

                    messagesQueue.TryDequeue(out _);
                }
            }
        }

        private void StopTask()
        {
            senderTask = default;

            messagesQueue.Clear();
        }

        #endregion Private Methods
    }
}