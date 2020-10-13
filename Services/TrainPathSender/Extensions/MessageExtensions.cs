using Common.Models;
using TrainPathImportService;

namespace TrainPathSender.Extensions
{
    internal static class MessageExtensions
    {
        #region Public Methods

        public static TrainPathStopStopPoint GetStopPoint(this TrainPathMessage message)
        {
            var result = new TrainPathStopStopPoint
            {
                @ref = message.Betriebsstelle,
            };

            return result;
        }

        #endregion Public Methods
    }
}