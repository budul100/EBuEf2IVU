using Common.Models;

namespace Message2TrainRunConverter.Extensions
{
    internal static class MessageExtensions
    {
        #region Public Methods

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

        #endregion Public Methods
    }
}