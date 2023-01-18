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
        private readonly string stoppingReasonPass;
        private readonly string stoppingReasonStop;
        private readonly string trainPathStateCancelled;
        private readonly string trainPathStateRun;

        private string sessionKey;
        private TimetableVersion timetableVersion;

        #endregion Private Fields

        #region Public Constructors

        public TrainRun2ImportPaths(string infrastructureManager, string orderingTransportationCompany,
            string stoppingReasonStop, string stoppingReasonPass, string trainPathStateRun,
            string trainPathStateCancelled, string importProfile, IEnumerable<string> locationShortnames)
        {
            this.infrastructureManager = infrastructureManager;
            this.orderingTransportationCompany = orderingTransportationCompany;
            this.stoppingReasonStop = stoppingReasonStop;
            this.stoppingReasonPass = stoppingReasonPass;
            this.trainPathStateRun = trainPathStateRun;
            this.trainPathStateCancelled = trainPathStateCancelled;
            this.importProfile = importProfile;
            this.locationShortnames = locationShortnames;
        }

        #endregion Public Constructors

        #region Public Methods

        public importTrainPaths Convert(IEnumerable<TrainRun> trainRuns)
        {
            var result = default(importTrainPaths);

            if (trainRuns.AnyItem())
            {
                var stopPoints = GetNetworkPointKeys(
                    trainRuns: trainRuns).ToArray();

                var trainPaths = GetTrainPaths(
                    trainRuns: trainRuns).ToArray();

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

        public void Initialize(DateTime ivuDatum, string sessionKey)
        {
            this.sessionKey = sessionKey;

            timetableVersion = GetTimetableVersion(ivuDatum);
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

        private TimetableVersion GetTimetableVersion(DateTime ivuDatum)
        {
            var validity = new DateInterval
            {
                begin = ivuDatum,
                bitmask = SingleDayBitmask,
                end = ivuDatum,
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
                @ref = timetableVersion?.id,
            };

            return result;
        }

        private TrainPathKey GetTrainPathKey(IEnumerable<TrainRun> trainRuns,
            TrainPathKeyTimetableVersion timetableVersionKey)
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
            var timetableVersionKey = GetTimetableVersionKey();

            var trainGroups = trainRuns
                .GroupBy(m => m.Zugnummer).ToArray();

            foreach (var trainGroup in trainGroups)
            {
                var trainPathKey = GetTrainPathKey(
                    trainRuns: trainGroup,
                    timetableVersionKey: timetableVersionKey);

                var trainPathVariants = GetTrainPathVariants(
                    trainRuns: trainGroup).ToArray();

                var result = new TrainPath
                {
                    importValidityFrame = timetableVersion?.validity.AsArray(),
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
                    && relevant != relevants[0];

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
                    running = !relevant.VerkehrNicht,
                    runningSpecified = true,
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
                    validity = timetableVersion?.validity.AsArray(),
                };

                yield return result;
            }
        }

        #endregion Private Methods
    }
}