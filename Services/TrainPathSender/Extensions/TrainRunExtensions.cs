using Common.Models;
using TrainPathImportService;

namespace TrainPathSender.Extensions
{
    internal static class TrainRunExtensions
    {
        #region Public Methods

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