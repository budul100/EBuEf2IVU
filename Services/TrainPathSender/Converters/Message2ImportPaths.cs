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
        private const string TimetableVersionId = "Timetable";

        private readonly Func<TrainPathMessage, DateTime> abfahrtGetter;
        private readonly Func<TrainPathMessage, DateTime> ankunftGetter;
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

        public Message2ImportPaths(DateTime sessionDate, string infrastructureManager,
            string orderingTransportationCompany, string stoppingReasonStop, string stoppingReasonPass,
            string importProfile, bool preferPrognosis)
        {
            this.sessionDate = sessionDate;
            this.infrastructureManager = infrastructureManager;
            this.orderingTransportationCompany = orderingTransportationCompany;

            this.stoppingReasonStop = stoppingReasonStop;
            this.stoppingReasonPass = stoppingReasonPass;
            this.importProfile = importProfile;

            abfahrtGetter = GetAbfahrtGetter(preferPrognosis);
            ankunftGetter = GetAnkunftGetter(preferPrognosis);

            timetableVersion = GetTimetableVersion();
            timetableVersionKey = GetTimetableVersionKey();
        }

        #endregion Public Constructors

        #region Public Methods

        public importTrainPaths Get(IEnumerable<TrainPathMessage> messages, string trainPathState)
        {
            var result = default(importTrainPaths);

            if (messages.AnyItem())
            {
                var stopPoints = GetNetworkPointKeys(messages).ToArray();
                var trainPaths = GetTrainPaths(
                    messages: messages,
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
                .Select(m => m.Betriebsstelle)
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
                trainPathId = relevant.Zugnummer,
            };

            return result;
        }

        private IEnumerable<TrainPath> GetTrainPaths(IEnumerable<TrainPathMessage> messages, string trainPathState)
        {
            var trainGroups = messages
                .GroupBy(m => m.Zugnummer).ToArray();

            foreach (var trainGroup in trainGroups)
            {
                var trainPathKey = GetTrainPathKey(trainGroup);
                var trainPathVariants = GetTrainPathVariants(
                    messages: trainGroup,
                    trainPathState: trainPathState).ToArray();

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
            foreach (var message in messages)
            {
                var times = new Times
                {
                    operationalArrivalTime = ankunftGetter.Invoke(message),
                    operationalArrivalTimeSpecified = message.AnkunftSoll.HasValue,
                    operationalDepartureTime = abfahrtGetter.Invoke(message),
                    operationalDepartureTimeSpecified = message.AbfahrtSoll.HasValue,
                };

                var stoppingReasons = GetStoppingReasons(message).ToArray();

                var result = new TrainPathStop
                {
                    arrivalTrack = message.GleisSoll?.ToString(),
                    departureTrack = message.GleisSoll?.ToString(),
                    running = true,
                    stoppingReasons = stoppingReasons,
                    stopPoint = message.GetStopPoint(),
                    times = times,
                };

                yield return result;
            }
        }

        private IEnumerable<TrainPathVariant> GetTrainPathVariants(IEnumerable<TrainPathMessage> messages, string trainPathState)
        {
            var variantGroups = messages
                .GroupBy(m => m.ZugId).ToArray();

            foreach (var variantGroup in variantGroups)
            {
                var trainPathItinerary = GetTrainPathStops(variantGroup).ToArray();

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