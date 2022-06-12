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

        public static TrainPosition GetPositionWithoutTraffic(this TrainPathMessage message)
        {
            var result = new TrainPosition
            {
                Abfahrt = message.AbfahrtPlan,
                Ankunft = message.AnkunftPlan,
                Bemerkungen = message.Bemerkungen,
                Betriebsstelle = message.Betriebsstelle,
                Gleis = message.GleisSoll?.ToString(),
                VerkehrNicht = true,
                IstDurchfahrt = message.IstDurchfahrt,
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