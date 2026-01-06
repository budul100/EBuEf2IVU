using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ConverterExtensions;
using EBuEf2IVU.Services.Message2LegConverter.Extensions;
using EBuEf2IVU.Services.Message2LegConverter.Settings;
using EBuEf2IVU.Shareds.Commons.Enums;
using EBuEf2IVU.Shareds.Commons.Interfaces;
using EBuEf2IVU.Shareds.Commons.Models;
using EnumerableExtensions;
using RegexExtensions;

namespace EBuEf2IVU.Services.Message2LegConverter
{
    public class Converter(IConfiguration config, ILogger<Converter> logger)
        : IMessage2LegConverter
    {
        #region Private Fields

        private readonly DateTime? dateMin = config.GetDateMin();
        private readonly IEnumerable<InfrastructureMapping> infrastructureMappings = config.GetInfrastructureMappings();
        private readonly ILogger logger = logger;

        #endregion Private Fields

        #region Public Methods

        public TrainLeg Convert(RealTimeMessage message)
        {
            var result = default(TrainLeg);

            if (message.SimulationsZeit.HasValue)
            {
                if (!dateMin.HasValue || message.SimulationsZeit.Value >= dateMin.Value)
                {
                    var mapping = infrastructureMappings
                        .Where(m => m?.MessageBetriebsstelle.IsMatch(message.Betriebsstelle) == true
                            && m.MessageStartGleis.IsMatchOrEmptyPatternOrEmptyValue(message.StartGleis)
                            && m.MessageEndGleis.IsMatchOrEmptyPatternOrEmptyValue(message.ZielGleis))
                        .OrderByDescending(m => m.MessageStartGleis.IsMatch(message.StartGleis))
                        .ThenByDescending(m => m.MessageEndGleis.IsMatch(message.ZielGleis)).FirstOrDefault();

                    if (mapping != default)
                    {
                        if (message.SignalTyp == SignalType.ESig && mapping.IVUTrainPositionType != LegType.Ankunft
                            || message.SignalTyp == SignalType.ASig && mapping.IVUTrainPositionType != LegType.Abfahrt
                            || message.SignalTyp == SignalType.BkSig && mapping.IVUTrainPositionType != LegType.Durchfahrt)
                        {
                            logger.LogWarning(
                                "Der IVUTrainPositionType des Mappings {mappingType} entspricht nicht " +
                                "dem SignalTyp der eingegangenen Nachricht: {message}",
                                mapping.IVUTrainPositionType,
                                message);
                        }
                    }

                    result = GetTrainLeg(
                        message: message,
                        mapping: mapping);
                }
                else if (dateMin.HasValue)
                {
                    logger.LogWarning(
                        "Die Simulationszeit der eingegangenen Nachricht liegt " +
                        "vor dem frühesten erlaubten Datum {dateMin}: {message}",
                        dateMin.Value,
                        message);
                }
            }

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private static TrainLeg GetTrainLeg(RealTimeMessage message, InfrastructureMapping mapping)
        {
            var ebuefZeitVon = mapping != default
                ? TimeSpan.FromSeconds(mapping.EBuEfVonVerschiebungSekunden.ToInt())
                : default;

            var ebuefZeitpunktVon = mapping != default
                ? message.SimulationsZeit.Value.Add(ebuefZeitVon).TimeOfDay
                : default;

            var ebuefZeitNach = mapping != default
                ? TimeSpan.FromSeconds(mapping.EBuEfNachVerschiebungSekunden.ToInt())
                : default;

            var ebuefZeitpunktNach = mapping != default
                ? message.SimulationsZeit.Value.Add(ebuefZeitNach).TimeOfDay
                : default;

            var ivuShift = mapping != default
                ? TimeSpan.FromSeconds(mapping.IVUVerschiebungSekunden.ToInt())
                : default;

            var ivuZeit = mapping != default
                ? message.SimulationsZeit.Value.Add(ivuShift).TimeOfDay
                : message.SimulationsZeit.Value.TimeOfDay;

            var fahrzeuge = message.Decoder.AsEnumerable();

            var result = new TrainLeg
            {
                EBuEfBetriebsstelleNach = mapping?.EBuEfNachBetriebsstelle,
                EBuEfBetriebsstelleVon = mapping?.EBuEfVonBetriebsstelle,
                EBuEfGleisNach = message.ZielGleis,
                EBuEfGleisVon = message.StartGleis,
                EBuEfZeitpunktNach = ebuefZeitpunktNach,
                EBuEfZeitpunktVon = ebuefZeitpunktVon,
                Fahrzeuge = fahrzeuge,
                IstPrognose = message.Modus == MessageType.Prognose,
                IVUGleis = mapping?.IVUGleis ?? message.ZielGleis,
                IVULegTyp = mapping?.IVUTrainPositionType ?? LegType.Abfahrt,
                IVUNetzpunkt = mapping?.IVUNetzpunkt ?? message.Betriebsstelle,
                IVUZeit = ivuZeit,
                Zugnummer = message.Zugnummer,
            };

            return result;
        }

        #endregion Private Methods
    }
}