using Common.Models;
using System;

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

        public static TrainPosition GetPositionWithTraffic(this TrainPathMessage message,
            Func<TrainPathMessage, DateTime?> abfahrtGetter, Func<TrainPathMessage, DateTime?> ankunftGetter)
        {
            var result = new TrainPosition
            {
                Abfahrt = abfahrtGetter.Invoke(message),
                Ankunft = ankunftGetter.Invoke(message),
                Bemerkungen = message.Bemerkungen,
                Betriebsstelle = message.Betriebsstelle,
                Gleis = message.GleisSoll?.ToString(),
                VerkehrNicht = false,
                IstDurchfahrt = message.IstDurchfahrt,
            };

            return result;
        }

        #endregion Public Methods
    }
}