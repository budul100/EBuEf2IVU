using Common.Interfaces;
using Common.Models;
using CredentialChannelFactory;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RealtimeSender.Converters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RealtimeSender
{
    public class Sender
        : IRealtimeSender
    {
        #region Private Fields

        private readonly ILogger logger;
        private readonly ConcurrentQueue<RealTimeInfoTO> messagesQueue = new ConcurrentQueue<RealTimeInfoTO>();

        private Factory<RealTimeInformationImportFacadeChannel> channelFactory;
        private Message2RealtimeInfo converter;
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

                cancellationToken.Register(() => StopTask());

                senderTask = retryPolicy?.ExecuteAsync(
                    action: (token) => RunSenderAsync(token),
                    cancellationToken: cancellationToken);
            }

            return senderTask;
        }

        public void Initialize(string host, int port, string path, string username, string password,
            bool isHttps, string division, int retryTime)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentException(
                    $"\"{nameof(host)}\" darf nicht NULL oder ein Leerraumzeichen sein.",
                    nameof(host));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(
                    $"\"{nameof(path)}\" kann nicht NULL oder leer sein.",
                    nameof(path));
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException(
                    $"\"{nameof(username)}\" kann nicht NULL oder leer sein.",
                    nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(
                    $"\"{nameof(password)}\" darf nicht NULL oder ein Leerraumzeichen sein.",
                    nameof(password));
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

            channelFactory = new Factory<RealTimeInformationImportFacadeChannel>(
                host: host,
                port: port,
                path: path,
                userName: username,
                password: password,
                isHttps: isHttps,
                notIgnoreCertificateErrors: true);

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
                "Fehler beim Senden der Ist-Zeiten an IVU.rail: {message}\r\n" +
                "Die Verbindung wird in {reconnection} Sekunden wieder versucht.",
                exception.Message,
                reconnection.TotalSeconds);
        }

        private async Task RunSenderAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!messagesQueue.IsEmpty)
                {
                    messagesQueue.TryPeek(out RealTimeInfoTO currentMessage);

                    if (currentMessage != default)
                    {
                        var importInfo = new importRealTimeInfo(new RealTimeInfoTO[] { currentMessage });
                        var sender = channelFactory.Get();

                        var response = await sender.importRealTimeInfoAsync(importInfo);
                        var result = response.importRealTimeInfoResponse1?.ToArray();

                        if (result?.Any() ?? false)
                        {
                            var relevantValiditation = result[0];

                            logger.LogError(
                                "Fehlermeldung zur Ist-Zeit-Nachricht von IVU.rail empfangen " +
                                "(Zug: {trainNumber}, Betriebsstelle: {location}, Decoder: {decoder}): {message}.",
                                currentMessage.tripNumber,
                                currentMessage.stopArea,
                                currentMessage.vehicles.FirstOrDefault()?.number,
                                relevantValiditation.message);
                        }
                        else
                        {
                            logger.LogDebug(
                                "Ist-Zeit-Nachricht wurde erfolgreich an IVU.rail gesendet " +
                                "(Zug: {trainNumber}, Betriebsstelle: {location}, Decoder: {decoder}).",
                                currentMessage.tripNumber,
                                currentMessage.stopArea,
                                currentMessage.vehicles.FirstOrDefault()?.number);
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