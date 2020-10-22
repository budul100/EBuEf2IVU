using Common.Models;
using EnumerableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using TrainPathImportService;
using TrainPathSender.Extensions;

namespace TrainPathSender.Converters
{
    internal class TrainRun2ImportPaths
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

        public TrainRun2ImportPaths(DateTime sessionDate, string infrastructureManager,
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
            var networkPoints = trainRuns
                .SelectMany(r => r.Positions)
                .Select(p => p.Betriebsstelle)
                .Distinct().ToArray();

            foreach (var networkPoint in networkPoints)
            {
                var result = new NetworkPointKey
                {
                    abbreviation = networkPoint,
                    id = networkPoint,
                };

                yield return result;
            }
        }

        private IEnumerable<string> GetStoppingReasons(TrainPosition trainPosition)
        {
            var result = trainPosition.IstDurchfahrt
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

        private TrainPathKey GetTrainPathKey(IEnumerable<TrainRun> trainRuns)
        {
            var relevant = trainRuns.First();

            var result = new TrainPathKey
            {
                infrastructureManager = infrastructureManager,
                timetableVersion = timetableVersionKey,
                trainPathId = relevant.ZugId.ToString(),
            };

            return result;
        }

        private IEnumerable<TrainPath> GetTrainPaths(IEnumerable<TrainRun> trainRuns, string trainPathState)
        {
            var trainGroups = trainRuns
                .GroupBy(m => m.Zugnummer).ToArray();

            foreach (var trainGroup in trainGroups)
            {
                var trainPathKey = GetTrainPathKey(trainGroup);
                var trainPathVariants = GetTrainPathVariants(
                    trainRuns: trainGroup,
                    trainPathState: trainPathState).ToArray();

                var result = new TrainPath
                {
                    importValidityFrame = timetableVersion.validity.AsArray(),
                    infrastructureManagerTrainPathId = trainGroup.Key.ToString(),
                    trainPathKey = trainPathKey,
                    trainPathVariants = trainPathVariants,
                };

                yield return result;
            }
        }

        private IEnumerable<TrainPathStop> GetTrainPathStops(TrainRun trainRun)
        {
            foreach (var trainPosition in trainRun.Positions)
            {
                var times = new Times
                {
                    operationalArrivalTime = trainPosition.Ankunft ?? default,
                    operationalArrivalTimeSpecified = trainPosition.Ankunft.HasValue,
                    operationalDepartureTime = trainPosition.Abfahrt ?? default,
                    operationalDepartureTimeSpecified = trainPosition.Abfahrt.HasValue,
                };

                var stoppingReasons = GetStoppingReasons(trainPosition).ToArray();

                var result = new TrainPathStop
                {
                    arrivalTrack = trainPosition.Gleis,
                    departureTrack = trainPosition.Gleis,
                    running = true,
                    stoppingReasons = stoppingReasons,
                    stopPoint = trainPosition.GetStopPoint(),
                    times = times,
                };

                yield return result;
            }
        }

        private IEnumerable<TrainPathVariant> GetTrainPathVariants(IEnumerable<TrainRun> trainRuns, string trainPathState)
        {
            foreach (var trainRun in trainRuns)
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
        }

        #endregion Private Methods
    }
}