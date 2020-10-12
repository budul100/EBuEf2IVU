#pragma warning disable CA1031 // Do not catch general exception types

using Common.Extensions;
using Common.Models;
using CredentialChannelFactory;
using DateTimeExtensions;
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

namespace TrainPathExchanger
{
    public class Sender
    {
        #region Private Fields

        private const string InfrastructureManager = "InfrastructureManager";
        private const string IVUImportProfile = "ImportProfile";
        private const string OrderingTransportationCompany = "orderingTransportationCompany";
        private const string SingleDayBitmask = "1";
        private const string StoppingReasonPass = "Passing";
        private const string StoppingReasonStop = "Stopping";
        private const string TimetableVersionId = "TimetableVersionId";
        private const string TrainPathState = "TrainPathState";

        private readonly ILogger<Sender> logger;
        private readonly ConcurrentQueue<TrainRun> runsQueue = new ConcurrentQueue<TrainRun>();

        private Factory<TrainPathImportWebFacadeChannel> channelFactory;
        private AsyncRetryPolicy retryPolicy;
        private DateTime sessionDate;
        private TimetableVersion timetableVersion;

        #endregion Private Fields

        #region Public Constructors

        public Sender(ILogger<Sender> logger)
        {
            this.logger = logger;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Initialize(string host, int port, string path, string username, string password, bool isHttps,
            int retryTime, DateTime sessionDate)
        {
            this.sessionDate = sessionDate;

            timetableVersion = GetTimetableVersion();

            channelFactory = new Factory<TrainPathImportWebFacadeChannel>(
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

        private importTrainPaths GetImportTrainPaths(TrainRun trainRun)
        {
            var result = default(importTrainPaths);

            if (trainRun != default)
            {
                var stopPoints = GetNetworkPointKeys(trainRun).ToArray();
                var trainPaths = GetTrainPaths(trainRun).ToArray();

                var request = new TrainPathImportRequest
                {
                    importProfile = IVUImportProfile,
                    stopPoints = stopPoints,
                    timetableVersions = GetTimetableVersions().ToArray(),
                    trainPaths = trainPaths,
                };

                result = new importTrainPaths
                {
                    trainPathImportRequest = request,
                };
            }

            return result;
        }

        private IEnumerable<NetworkPointKey> GetNetworkPointKeys(TrainRun trainRun)
        {
            foreach (var position in trainRun.Positions)
            {
                var result = new NetworkPointKey
                {
                    abbreviation = position.Betriebsstelle,
                    id = position.Betriebsstelle,
                };

                yield return result;
            }
        }

        private IEnumerable<string> GetStoppingReasons(TrainPosition position)
        {
            var result = position.IstDurchfahrt
                ? StoppingReasonPass
                : StoppingReasonStop;

            yield return result;
        }

        private TimetableVersion GetTimetableVersion()
        {
            var validity = new DateInterval
            {
                begin = sessionDate,
                bitmask = SingleDayBitmask,
                end = sessionDate
            };

            var result = new TimetableVersion
            {
                id = TimetableVersionId,
                validity = validity,
            };
            return result;
        }

        private IEnumerable<TimetableVersion> GetTimetableVersions()
        {
            yield return timetableVersion;
        }

        private IEnumerable<TrainPath> GetTrainPaths(TrainRun trainRun)
        {
            var keyTimetableVersion = new TrainPathKeyTimetableVersion
            {
                @ref = timetableVersion.id,
            };

            var trainPathKey = new TrainPathKey
            {
                infrastructureManager = InfrastructureManager,
                timetableVersion = keyTimetableVersion,
                trainPathId = trainRun.Zugnummer,
            };

            var trainPathVariants = GetTrainPathVariants(trainRun).ToArray();

            var result = new TrainPath
            {
                importValidityFrame = GetValidities().ToArray(),
                infrastructureManagerTrainPathId = trainRun.Zugnummer,
                trainPathKey = trainPathKey,
                trainPathVariants = trainPathVariants,
            };

            yield return result;
        }

        private IEnumerable<TrainPathStop> GetTrainPathStops(TrainRun trainRun)
        {
            foreach (var position in trainRun.Positions)
            {
                var times = new Times
                {
                    operationalArrivalTime = position.Ankunft?.ToDateTime() ?? DateTime.Now,
                    operationalArrivalTimeSpecified = position.Ankunft.HasValue,
                    operationalDepartureTime = position.Abfahrt?.ToDateTime() ?? DateTime.Now,
                    operationalDepartureTimeSpecified = position.Abfahrt.HasValue,
                };

                var stopPoint = new TrainPathStopStopPoint
                {
                    @ref = position.Betriebsstelle,
                };

                var stoppingReasons = GetStoppingReasons(position).ToArray();

                var result = new TrainPathStop
                {
                    arrivalTrack = position.Gleis,
                    departureTrack = position.Gleis,
                    running = true,
                    stoppingReasons = stoppingReasons,
                    stopPoint = stopPoint,
                    times = times,
                };

                yield return result;
            }
        }

        private IEnumerable<TrainPathVariant> GetTrainPathVariants(TrainRun trainRun)
        {
            var trainPathItinerary = GetTrainPathStops(trainRun).ToArray();

            var result = new TrainPathVariant
            {
                orderingTransportationCompany = OrderingTransportationCompany,
                state = TrainPathState,
                trainDescription = trainRun.Bemerkungen,
                trainPathItinerary = trainPathItinerary,
                validity = GetValidities().ToArray(),
            };

            yield return result;
        }

        private IEnumerable<DateInterval> GetValidities()
        {
            yield return timetableVersion.validity;
        }

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
                if (!runsQueue.IsEmpty)
                {
                    var currentRun = runsQueue.GetFirst().FirstOrDefault();
                    var importPaths = GetImportTrainPaths(currentRun);

                    if (importPaths != default)
                    {
                        using var channel = channelFactory.Get();
                        var response = await channel.importTrainPathsAsync(importPaths);

                        if (response.trainPathImportResponse != default)
                        {
                            logger.LogDebug(
                                "Trasse {trainNumber} wurde erfolgreich an IVU.rail mit folgender id gesendet: {id}",
                                currentRun.Zugnummer,
                                response.trainPathImportResponse.protocolTransactionId);
                        }

                        runsQueue.TryDequeue(out TrainRun _);
                    }
                }
            }
        }

        #endregion Private Methods
    }
}

#pragma warning disable CA1031 // Do not catch general exception types