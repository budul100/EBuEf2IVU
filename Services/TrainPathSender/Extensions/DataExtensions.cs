using Common.Models;
using TrainPathImportService;

namespace TrainPathSender.Extensions
{
    internal static class DataExtensions
    {
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

        #endregion Public Methods
    }
}