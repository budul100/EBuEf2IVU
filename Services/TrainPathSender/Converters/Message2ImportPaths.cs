using Common.Models;
using EnumerableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using TrainPathImportService;
using TrainPathSender.Extensions;

namespace TrainPathSender.Converters
{
    internal class Message2ImportPaths
    {
        #region Private Fields

        private const string SingleDayBitmask = "1";
        private const string StopType = "STOP_POST";
        private const string TimetableVersionId = "Timetable";

        private readonly Func<TrainPathMessage, DateTime> abfahrtGetter;
        private readonly Func<TrainPathMessage, DateTime> ankunftGetter;
        private readonly string importProfile;
        private readonly string infrastructureManager;
        private readonly IEnumerable<string> locationShortnames;
        private readonly string orderingTransportationCompany;
        private readonly DateTime sessionDate;
        private readonly string stoppingReasonPass;
        private readonly string stoppingReasonStop;
        private readonly TimetableVersion timetableVersion;
        private readonly TrainPathKeyTimetableVersion timetableVersionKey;
        private readonly string trainPathStateCancelled;
        private readonly string trainPathStateRun;

        #endregion Private Fields

        #region Public Constructors

        public Message2ImportPaths(DateTime sessionDate, string infrastructureManager,
            string orderingTransportationCompany, string stoppingReasonStop, string stoppingReasonPass,
            string trainPathStateRun, string trainPathStateCancelled, string importProfile, bool preferPrognosis,
            IEnumerable<string> locationShortnames)
        {
            this.sessionDate = sessionDate;
            this.infrastructureManager = infrastructureManager;
            this.orderingTransportationCompany = orderingTransportationCompany;

            this.stoppingReasonStop = stoppingReasonStop;
            this.stoppingReasonPass = stoppingReasonPass;
            this.trainPathStateRun = trainPathStateRun;
            this.trainPathStateCancelled = trainPathStateCancelled;
            this.importProfile = importProfile;
            this.locationShortnames = locationShortnames;

            abfahrtGetter = GetAbfahrtGetter(preferPrognosis);
            ankunftGetter = GetAnkunftGetter(preferPrognosis);

            timetableVersion = GetTimetableVersion();
            timetableVersionKey = GetTimetableVersionKey();
        }

        #endregion Public Constructors

        #region Public Methods

        public importTrainPaths Get(IEnumerable<TrainPathMessage> messages)
        {
            var result = default(importTrainPaths);

            if (messages.AnyItem())
            {
                var stopPoints = GetNetworkPointKeys(messages).ToArray();
                var trainPaths = GetTrainPaths(messages).ToArray();

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

        private static Func<TrainPathMessage, DateTime> GetAbfahrtGetter(bool preferPrognosis)
        {
            var result = preferPrognosis
                ? (Func<TrainPathMessage, DateTime>)(m => m.AbfahrtPrognose ?? m.AbfahrtSoll ?? DateTime.MinValue)
                : (Func<TrainPathMessage, DateTime>)(m => m.AbfahrtSoll ?? DateTime.MinValue);

            return result;
        }

        private static Func<TrainPathMessage, DateTime> GetAnkunftGetter(bool preferPrognosis)
        {
            var result = preferPrognosis
                ? (Func<TrainPathMessage, DateTime>)(m => m.AnkunftPrognose ?? m.AnkunftSoll ?? DateTime.MinValue)
                : (Func<TrainPathMessage, DateTime>)(m => m.AnkunftSoll ?? DateTime.MinValue);

            return result;
        }

        private IEnumerable<NetworkPointKey> GetNetworkPointKeys(IEnumerable<TrainPathMessage> messages)
        {
            var networkPoints = messages
                .Select(m => m.Betriebsstelle).Distinct()
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

        private IEnumerable<string> GetStoppingReasons(TrainPathMessage message)
        {
            var result = message.IstDurchfahrt
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

        private TrainPathKey GetTrainPathKey(IEnumerable<TrainPathMessage> messages)
        {
            var relevant = messages.First();

            var result = new TrainPathKey
            {
                infrastructureManager = infrastructureManager,
                timetableVersion = timetableVersionKey,
                trainPathId = relevant.ZugId.ToString(),
            };

            return result;
        }

        private IEnumerable<TrainPath> GetTrainPaths(IEnumerable<TrainPathMessage> messages)
        {
            var trainGroups = messages
                .GroupBy(m => m.Zugnummer).ToArray();

            foreach (var trainGroup in trainGroups)
            {
                var trainPathKey = GetTrainPathKey(trainGroup);
                var trainPathVariants = GetTrainPathVariants(trainGroup).ToArray();

                var result = new TrainPath
                {
                    importValidityFrame = timetableVersion.validity.AsArray(),
                    infrastructureManagerTrainPathId = trainGroup.Key,
                    trainPathKey = trainPathKey,
                    trainPathVariants = trainPathVariants,
                };

                yield return result;
            }
        }

        private IEnumerable<TrainPathStop> GetTrainPathStops(IEnumerable<TrainPathMessage> messages)
        {
            var relevants = messages
                .Where(m => !locationShortnames.AnyItem() || locationShortnames.Contains(m.Betriebsstelle)).ToArray();

            foreach (var relevant in relevants)
            {
                var times = new Times
                {
                    operationalArrivalTime = ankunftGetter.Invoke(relevant),
                    operationalArrivalTimeTextSpecified = relevant.AnkunftSoll.HasValue,
                    operationalDepartureTime = abfahrtGetter.Invoke(relevant),
                    operationalDepartureTimeTextSpecified = relevant.AbfahrtSoll.HasValue,
                };

                var stoppingReasons = GetStoppingReasons(relevant).ToArray();

                var isRunning = relevant.IsRunning();

                var result = new TrainPathStop
                {
                    arrivalTrack = relevant.GleisSoll?.ToString(),
                    departureTrack = relevant.GleisSoll?.ToString(),
                    running = isRunning,
                    runningSpecified = true,
                    stoppingReasons = stoppingReasons,
                    stopPoint = relevant.GetStopPoint(),
                    times = times,
                };

                yield return result;
            }
        }

        private IEnumerable<TrainPathVariant> GetTrainPathVariants(IEnumerable<TrainPathMessage> messages)
        {
            var variantGroups = messages
                .GroupBy(m => m.ZugId).ToArray();

            foreach (var variantGroup in variantGroups)
            {
                var trainPathItinerary = GetTrainPathStops(variantGroup).ToArray();

                var isRunning = trainPathItinerary
                    .Where(i => i.running).Any();

                var trainPathState = isRunning
                    ? trainPathStateRun
                    : trainPathStateCancelled;

                var result = new TrainPathVariant
                {
                    orderingTransportationCompany = orderingTransportationCompany,
                    state = trainPathState,
                    trainDescription = variantGroup.First().Bemerkungen,
                    trainPathItinerary = trainPathItinerary,
                    validity = timetableVersion.validity.AsArray(),
                };

                yield return result;
            }
        }

        #endregion Private Methods
    }
}