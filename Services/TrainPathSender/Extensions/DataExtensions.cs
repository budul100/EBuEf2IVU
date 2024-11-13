using System;
using Commons.Models;
using EnumerableExtensions;
using TrainPathImportService110;

namespace TrainPathSender.Extensions
{
    internal static class DataExtensions
    {
        #region Private Fields

        private const string SingleDayBitmask = "1";
        private const string TimetableVersionId = "Timetable";
        private const string ValueSeparator = ", ";

        #endregion Private Fields

        #region Public Methods

        public static ItinerarySegmentAttributes GetItinerarySegmentAttributes(this TrainRun trainRun)
        {
            var result = new ItinerarySegmentAttributes
            {
                trainNumber = trainRun.Zugnummer.ToString(),
                trainProduct = trainRun.Zuggattung,
            };

            return result;
        }

        public static TrainPathStopStopPoint GetStopPoint(this TrainPosition position)
        {
            var result = new TrainPathStopStopPoint
            {
                @ref = position.Betriebsstelle,
            };

            return result;
        }

        public static TimetableVersion GetTimetableVersion(this DateTime ivuDatum)
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

        public static State GetTrainPathState(this string value)
        {
            foreach (State state in Enum.GetValues(typeof(State)))
            {
                if (state.ToString() == value)
                {
                    return state;
                }
            }

            var allStates = string.Join(
                separator: ValueSeparator,
                values: Enum.GetValues(typeof(State)));

            throw new ApplicationException(
                $"The train path state '{value}' is wrong. It must have one of the following values: {allStates}.");
        }

        #endregion Public Methods
    }
}