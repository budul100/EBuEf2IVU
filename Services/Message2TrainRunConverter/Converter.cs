using Commons.Interfaces;
using Commons.Models;
using EnumerableExtensions;
using Message2TrainRunConverter.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Message2TrainRunConverter
{
    public class Converter
        : IMessage2TrainRunConverter
    {
        #region Private Fields

        private readonly Func<TrainPathMessage, DateTime?> abfahrtGetter;
        private readonly Func<TrainPathMessage, DateTime?> ankunftGetter;

        #endregion Private Fields

        #region Public Constructors

        public Converter(IConfiguration config)
        {
            abfahrtGetter = config.GetAbfahrtGetter();
            ankunftGetter = config.GetAnkunftGetter();
        }

        #endregion Public Constructors

        #region Public Methods

        public IEnumerable<TrainRun> Convert(IEnumerable<TrainPathMessage> messages)
        {
            if (messages.AnyItem())
            {
                var trainGroups = messages
                    .GroupBy(m => m.ZugId).ToArray();

                foreach (var trainGroup in trainGroups)
                {
                    var result = GetTrainRun(trainGroup);

                    if (result != default)
                    {
                        yield return result;
                    }
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        private IEnumerable<TrainPosition> GetPositions(IEnumerable<TrainPathMessage> messages)
        {
            foreach (var message in messages)
            {
                if (message.AnkunftSoll == default
                    && message.AbfahrtSoll == default)
                {
                    yield return message.GetPositionWithoutTraffic();
                }
                else
                {
                    var result = message.GetPositionWithTraffic(
                        abfahrtGetter: abfahrtGetter,
                        ankunftGetter: ankunftGetter);

                    yield return result;
                }
            }
        }

        private TrainRun GetTrainRun(IEnumerable<TrainPathMessage> messages)
        {
            var result = default(TrainRun);

            var positions = GetPositions(messages).ToArray();

            if (positions.Length > 1)
            {
                var relevantMessage = messages.First();

                result = new TrainRun
                {
                    Abfahrt = positions[0].Abfahrt?.TimeOfDay,
                    IstGeaendert = true,
                    Positions = positions,
                    Zuggattung = relevantMessage.Zuggattung,
                    ZugId = relevantMessage.ZugId,
                    Zugnummer = relevantMessage.Zugnummer,
                };
            }

            return result;
        }

        #endregion Private Methods
    }
}