#pragma warning disable CA1031 // Do not catch general exception types

using Common.Extensions;
using Common.Interfaces;
using Common.Models;
using CredentialChannelFactory;
using DateTimeExtensions;
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

namespace TrainPathSender
{
    public class Sender
        : ITrainPathSender
    {
        #region Private Fields

        private const string SingleDayBitmask = "1";
        private const string TimetableVersionId = "Timetable";

        private readonly ConcurrentQueue<importTrainPaths> importsQueue = new ConcurrentQueue<importTrainPaths>();
        private readonly ILogger<Sender> logger;

        private Factory<TrainPathImportWebFacadeChannel> channelFactory;
        private string importProfile;
        private string infrastructureManager;
        private string orderingTransportationCompany;
        private AsyncRetryPolicy retryPolicy;
        private DateTime sessionDate;
        private string stoppingReasonPass;
        private string stoppingReasonStop;
        private TimetableVersion timetableVersion;
        private TrainPathKeyTimetableVersion timetableVersionKey;
        private string trainPathState;

        #endregion Private Fields

        #region Public Constructors

        public Sender(ILogger<Sender> logger)
        {
            this.logger = logger;
        }

        #endregion Public Constructors

        #region Public Methods

        public void AddTrain(TrainRun trainRun)
        {
            if (trainRun != default)
            {
                AddTrains(trainRun.AsEnumerable());
            }
        }

        public void AddTrains(IEnumerable<TrainRun> trainRuns)
        {
            if (trainRuns.AnyItem())
            {
                var imports = GetImportTrainPaths(trainRuns);

                if (imports != default)
                    importsQueue.Enqueue(imports);
            }
        }

        public void Initialize(string host, int port, string path, string username, string password, bool isHttps,
            int retryTime, DateTime sessionDate, string infrastructureManager, string orderingTransportationCompany,
            string trainPathState, string stoppingReasonStop, string stoppingReasonPass, string importProfile)
        {
            this.sessionDate = sessionDate;
            this.infrastructureManager = infrastructureManager;
            this.orderingTransportationCompany = orderingTransportationCompany;

            this.trainPathState = trainPathState;
            this.stoppingReasonStop = stoppingReasonStop;
            this.stoppingReasonPass = stoppingReasonPass;
            this.importProfile = importProfile;

            timetableVersion = GetTimetableVersion();
            timetableVersionKey = GetTimetableVersionKey();

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

        private importTrainPaths GetImportTrainPaths(IEnumerable<TrainRun> trainRuns)
        {
            var result = default(importTrainPaths);

            if (trainRuns.AnyItem())
            {
                var stopPoints = GetNetworkPointKeys(trainRuns).ToArray();
                var trainPaths = GetTrainPaths(trainRuns).ToArray();

                var request = new TrainPathImportRequest
                {
                    importProfile = importProfile,
                    stopPoints = stopPoints,
                    timetableVersions = timetableVersion.AsArray(),
                    trainPaths = trainPaths,
                };

                result = new importTrainPaths
                {
                    trainPathImportRequest = request,
                };
            }

            return result;
        }

        private IEnumerable<NetworkPointKey> GetNetworkPointKeys(IEnumerable<TrainRun> trainRuns)
        {
            var positions = trainRuns
                .SelectMany(r => r.Positions)
                .Distinct().ToArray();

            foreach (var position in positions)
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
                ? stoppingReasonPass
                : stoppingReasonStop;

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

        private TrainPathKeyTimetableVersion GetTimetableVersionKey()
        {
            var result = new TrainPathKeyTimetableVersion
            {
                @ref = timetableVersion.id,
            };

            return result;
        }

        private TrainPathKey GetTrainPathKey(TrainRun trainRun)
        {
            var result = new TrainPathKey
            {
                infrastructureManager = infrastructureManager,
                timetableVersion = timetableVersionKey,
                trainPathId = trainRun.Zugnummer,
            };

            return result;
        }

        private IEnumerable<TrainPath> GetTrainPaths(IEnumerable<TrainRun> trainRuns)
        {
            foreach (var trainRun in trainRuns.IfAny())
            {
                var trainPathKey = GetTrainPathKey(trainRun);
                var trainPathVariants = GetTrainPathVariants(trainRun).ToArray();

                var result = new TrainPath
                {
                    importValidityFrame = timetableVersion.validity.AsArray(),
                    infrastructureManagerTrainPathId = trainRun.Zugnummer,
                    trainPathKey = trainPathKey,
                    trainPathVariants = trainPathVariants,
                };

                yield return result;
            }
        }

        private IEnumerable<TrainPathStop> GetTrainPathStops(TrainRun trainRun)
        {
            foreach (var position in trainRun.Positions)
            {
                var times = new Times
                {
                    operationalArrivalTime = position.Ankunft.ToDateTime() ?? DateTime.Now,
                    operationalArrivalTimeSpecified = position.Ankunft.HasValue,
                    operationalDepartureTime = position.Abfahrt.ToDateTime() ?? DateTime.Now,
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
                orderingTransportationCompany = orderingTransportationCompany,
                state = trainPathState,
                trainDescription = trainRun.Bemerkungen,
                trainPathItinerary = trainPathItinerary,
                validity = timetableVersion.validity.AsArray(),
            };

            yield return result;
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
                if (!importsQueue.IsEmpty)
                {
                    var currentImport = importsQueue.GetFirst().FirstOrDefault();

                    if (currentImport != default)
                    {
                        using var channel = channelFactory.Get();
                        var response = await channel.importTrainPathsAsync(currentImport);

                        if (response.trainPathImportResponse != default)
                        {
                            logger.LogDebug(
                                "Trassen wurden mit folgender ID an IVU.rail gesendet: {id}",
                                response.trainPathImportResponse.protocolTransactionId);
                        }

                        importsQueue.TryDequeue(out importTrainPaths _);
                    }
                }
            }
        }

        #endregion Private Methods
    }
}

#pragma warning disable CA1031 // Do not catch general exception types