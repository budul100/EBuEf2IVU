using Common.Models;
using EnumerableExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TrainPathSender.Converters
{
    internal class Message2TrainRun
    {
        #region Private Fields

        private readonly Func<TrainPathMessage, DateTime> abfahrtGetter;
        private readonly Func<TrainPathMessage, DateTime> ankunftGetter;

        #endregion Private Fields

        #region Public Constructors

        public Message2TrainRun(bool preferPrognosis)
        {
            abfahrtGetter = GetAbfahrtGetter(preferPrognosis);
            ankunftGetter = GetAnkunftGetter(preferPrognosis);
        }

        #endregion Public Constructors

        #region Public Methods

        public IEnumerable<TrainRun> Get(IEnumerable<TrainPathMessage> messages)
        {
            if (messages.AnyItem())
            {
                var trainGroups = messages
                    .GroupBy(m => m.ZugId).ToArray();

                foreach (var trainGroup in trainGroups)
                {
                    var result = GetTrainRun(trainGroup);

                    if (result.Positions.Count() > 1)
                    {
                        yield return result;
                    }
                }
            }
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

        private IEnumerable<TrainPosition> GetPositions(IEnumerable<TrainPathMessage> messages)
        {
            foreach (var message in messages)
            {
                var result = new TrainPosition
                {
                    Abfahrt = abfahrtGetter.Invoke(message),
                    Ankunft = ankunftGetter.Invoke(message),
                    Bemerkungen = message.Bemerkungen,
                    Betriebsstelle = message.Betriebsstelle,
                    Gleis = message.GleisSoll?.ToString(),
                    IstDurchfahrt = message.IstDurchfahrt,
                };

                yield return result;
            }
        }

        private TrainRun GetTrainRun(IEnumerable<TrainPathMessage> messages)
        {
            var positions = GetPositions(messages).ToArray();

            var relevantMessage = messages.First();

            var result = new TrainRun
            {
                Abfahrt = positions.First().Abfahrt?.TimeOfDay,
                Positions = positions,
                Zuggattung = relevantMessage.Zuggattung,
                ZugId = relevantMessage.ZugId,
                Zugnummer = relevantMessage.Zugnummer,
            };

            return result;
        }

        #endregion Private Methods
    }
}