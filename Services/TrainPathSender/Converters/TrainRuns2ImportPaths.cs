using Common.Models;
using DateTimeExtensions;
using EnumerableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using TrainPathImportService;

namespace TrainPathSender.Converters
{
    internal class TrainRuns2ImportPaths
    {
        #region Private Fields

        private const string SingleDayBitmask = "1";
        private const string TimetableVersionId = "Timetable";

        private readonly string importProfile;
        private readonly string infrastructureManager;
        private readonly string orderingTransportationCompany;
        private readonly DateTime sessionDate;
        private readonly string stoppingReasonPass;
        private readonly string stoppingReasonStop;
        private readonly TimetableVersion timetableVersion;
        private readonly TrainPathKeyTimetableVersion timetableVersionKey;

        #endregion Private Fields

        #region Public Constructors

        public TrainRuns2ImportPaths(DateTime sessionDate, string infrastructureManager,
            string orderingTransportationCompany, string stoppingReasonStop, string stoppingReasonPass,
            string importProfile)
        {
            this.sessionDate = sessionDate;
            this.infrastructureManager = infrastructureManager;
            this.orderingTransportationCompany = orderingTransportationCompany;

            this.stoppingReasonStop = stoppingReasonStop;
            this.stoppingReasonPass = stoppingReasonPass;
            this.importProfile = importProfile;

            timetableVersion = GetTimetableVersion();
            timetableVersionKey = GetTimetableVersionKey();
        }

        #endregion Public Constructors

        #region Public Methods

        public importTrainPaths Get(IEnumerable<TrainRun> trainRuns, string trainPathState)
        {
            var result = default(importTrainPaths);

            if (trainRuns.AnyItem())
            {
                var stopPoints = GetNetworkPointKeys(trainRuns).ToArray();
                var trainPaths = GetTrainPaths(
                    trainRuns: trainRuns,
                    trainPathState: trainPathState).ToArray();

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

        #endregion Public Methods

        #region Private Methods

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

        private IEnumerable<TrainPath> GetTrainPaths(IEnumerable<TrainRun> trainRuns, string trainPathState)
        {
            foreach (var trainRun in trainRuns.IfAny())
            {
                var trainPathKey = GetTrainPathKey(trainRun);
                var trainPathVariants = GetTrainPathVariants(
                    trainRun: trainRun,
                    trainPathState: trainPathState).ToArray();

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

        private IEnumerable<TrainPathVariant> GetTrainPathVariants(TrainRun trainRun, string trainPathState)
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

        #endregion Private Methods
    }
}