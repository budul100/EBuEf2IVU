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
        private const string StopType = "STOP_POST";
        private const string TimetableVersionId = "Timetable";

        private readonly string importProfile;
        private readonly string infrastructureManager;
        private readonly IEnumerable<string> locationShortnames;
        private readonly string orderingTransportationCompany;
        private readonly DateTime sessionDate;
        private readonly string sessionKey;
        private readonly string stoppingReasonPass;
        private readonly string stoppingReasonStop;
        private readonly TimetableVersion timetableVersion;
        private readonly TrainPathKeyTimetableVersion timetableVersionKey;
        private readonly string trainPathStateCancelled;
        private readonly string trainPathStateRun;

        #endregion Private Fields

        #region Public Constructors

        public TrainRun2ImportPaths(DateTime sessionDate, string sessionKey, string infrastructureManager,
            string orderingTransportationCompany, string stoppingReasonStop, string stoppingReasonPass,
            string trainPathStateRun, string trainPathStateCancelled, string importProfile,
            IEnumerable<string> locationShortnames)
        {
            this.sessionDate = sessionDate;
            this.sessionKey = sessionKey;
            this.infrastructureManager = infrastructureManager;
            this.orderingTransportationCompany = orderingTransportationCompany;
            this.stoppingReasonStop = stoppingReasonStop;
            this.stoppingReasonPass = stoppingReasonPass;
            this.trainPathStateRun = trainPathStateRun;
            this.trainPathStateCancelled = trainPathStateCancelled;
            this.importProfile = importProfile;
            this.locationShortnames = locationShortnames;

            timetableVersion = GetTimetableVersion();
            timetableVersionKey = GetTimetableVersionKey();
        }

        #endregion Public Constructors

        #region Public Methods

        public importTrainPaths Get(IEnumerable<TrainRun> trainRuns)
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

        #endregion Public Methods

        #region Private Methods

        private IEnumerable<NetworkPointKey> GetNetworkPointKeys(IEnumerable<TrainRun> trainRuns)
        {
            var networkPoints = trainRuns
                .SelectMany(r => r.Positions)
                .Select(p => p.Betriebsstelle).Distinct()
                .Where(b => !locationShortnames.AnyItem() || locationShortnames.Contains(b)).ToArray();

            foreach (var networkPoint in networkPoints)
            {
                var result = new NetworkPointKey
                {
                    abbreviation = networkPoint,
                    id = networkPoint,
                    stopType = StopType,
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
            var trainPathId = $"{relevant.ZugId}_{sessionKey}";

            var result = new TrainPathKey
            {
                infrastructureManager = infrastructureManager,
                timetableVersion = timetableVersionKey,
                trainPathId = trainPathId,
            };

            return result;
        }

        private IEnumerable<TrainPath> GetTrainPaths(IEnumerable<TrainRun> trainRuns)
        {
            var trainGroups = trainRuns
                .GroupBy(m => m.Zugnummer).ToArray();

            foreach (var trainGroup in trainGroups)
            {
                var trainPathKey = GetTrainPathKey(trainGroup);
                var trainPathVariants = GetTrainPathVariants(trainGroup).ToArray();

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
            var relevants = trainRun.Positions
                .Where(p => !locationShortnames.AnyItem() || locationShortnames.Contains(p.Betriebsstelle)).ToArray();

            var itinerarySegmentAttributes = trainRun.GetItinerarySegmentAttributes();

            foreach (var relevant in relevants)
            {
                var arrivalSpecified = relevant.Ankunft.HasValue
                    && relevant != relevants.First();

                var departureSpecified = relevant.Abfahrt.HasValue
                    && relevant != relevants.Last();

                var times = new Times
                {
                    operationalArrivalTime = relevant.Ankunft ?? default,
                    operationalArrivalTimeTextSpecified = arrivalSpecified,
                    operationalDepartureTime = relevant.Abfahrt ?? default,
                    operationalDepartureTimeTextSpecified = departureSpecified,
                };

                var stoppingReasons = GetStoppingReasons(relevant).ToArray();

                var arrivalTrack = arrivalSpecified
                    ? relevant.Gleis?.ToString()
                    : default;

                var departureTrack = departureSpecified
                    ? relevant.Gleis?.ToString()
                    : default;

                var result = new TrainPathStop
                {
                    arrivalTrack = arrivalTrack,
                    departureTrack = departureTrack,
                    running = relevant.IstVorhanden,
                    stoppingReasons = stoppingReasons,
                    stopPoint = relevant.GetStopPoint(),
                    times = times,
                    itinerarySegmentAttributes = itinerarySegmentAttributes,
                };

                yield return result;
            }
        }

        private IEnumerable<TrainPathVariant> GetTrainPathVariants(IEnumerable<TrainRun> trainRuns)
        {
            foreach (var trainRun in trainRuns)
            {
                var trainPathItinerary = GetTrainPathStops(trainRun).ToArray();

                var state = trainPathItinerary.Any(i => i.running)
                    ? trainPathStateRun
                    : trainPathStateCancelled;

                var result = new TrainPathVariant
                {
                    orderingTransportationCompany = orderingTransportationCompany,
                    state = state,
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