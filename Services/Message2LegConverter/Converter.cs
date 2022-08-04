using Common.Enums;
using Common.Models;
using ConverterExtensions;
using EBuEf2IVUVehicle.Settings;
using EnumerableExtensions;
using Message2LegConverter.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RegexExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Message2LegConverter
{
    public class Converter
        : IMessage2LegConverter
    {
        #region Private Fields

        private readonly IEnumerable<InfrastructureMapping> infrastructureMappings;
        private readonly ILogger logger;

        private DateTime ivuSessionDate;

        #endregion Private Fields

        #region Public Constructors

        public Converter(IConfiguration config, ILogger<Converter> logger)
        {
            this.logger = logger;

            infrastructureMappings = config.GetInfrastructureMappings();
        }

        #endregion Public Constructors

        #region Public Methods

        public TrainLeg Convert(RealTimeMessage message)
        {
            var mapping = default(InfrastructureMapping);

            if (message.SimulationsZeit.HasValue)
            {
                mapping = infrastructureMappings
                    .Where(m => m?.MessageBetriebsstelle.IsMatch(message.Betriebsstelle) == true
                        && m.MessageStartGleis.IsMatchOrEmptyPatternOrEmptyValue(message.StartGleis)
                        && m.MessageEndGleis.IsMatchOrEmptyPatternOrEmptyValue(message.EndGleis))
                    .OrderByDescending(m => m.MessageStartGleis.IsMatch(message.StartGleis))
                    .ThenByDescending(m => m.MessageEndGleis.IsMatch(message.EndGleis)).FirstOrDefault();
            }

            if (mapping != default)
            {
                if ((message.SignalTyp == SignalType.ESig && mapping.IVUTrainPositionType != LegType.Ankunft)
                    || (message.SignalTyp == SignalType.ASig && mapping.IVUTrainPositionType != LegType.Abfahrt)
                    || (message.SignalTyp == SignalType.BkSig && mapping.IVUTrainPositionType != LegType.Durchfahrt))
                {
                    logger.LogWarning(
                        "Der IVUTrainPositionType des Mappings {mappingType} entspricht nicht " +
                        "dem SignalTyp der eingegangenen Nachricht: {message}",
                        mapping.IVUTrainPositionType,
                        message);
                }

                return GetTrainLeg(
                    message: message,
                    mapping: mapping);
            }
            else
            {
                return GetTrainLeg(
                    message: message);
            }
        }

        public void Initialize(DateTime ivuSessionDate)
        {
            this.ivuSessionDate = ivuSessionDate.Date;
        }

        #endregion Public Methods

        #region Private Methods

        private TrainLeg GetTrainLeg(RealTimeMessage message)
        {
            var ivuZeitpunkt = message.SimulationsZeit.Value.TimeOfDay;
            var fahrzeuge = message?.Decoder.AsEnumerable();

            var result = new TrainLeg
            {
                Fahrzeuge = fahrzeuge,
                IstPrognose = message.Modus == MessageType.Prognose,
                IVUGleis = message.ZielGleis,
                IVULegTyp = LegType.Ankunft,
                IVUNetzpunkt = message.Betriebsstelle,
                IVUZeitpunkt = ivuSessionDate.Add(ivuZeitpunkt),
                Zugnummer = message.Zugnummer,
            };

            return result;
        }

        private TrainLeg GetTrainLeg(RealTimeMessage message, InfrastructureMapping mapping)
        {
            var ebuefZeitVon = TimeSpan.FromSeconds(mapping.EBuEfVonVerschiebungSekunden.ToInt());
            var ebuefZeitpunktVon = message.SimulationsZeit.Value.Add(ebuefZeitVon).TimeOfDay;

            var ebuefZeitNach = TimeSpan.FromSeconds(mapping.EBuEfNachVerschiebungSekunden.ToInt());
            var ebuefZeitpunktNach = message.SimulationsZeit.Value.Add(ebuefZeitNach).TimeOfDay;

            var ivuZeit = TimeSpan.FromSeconds(mapping.IVUVerschiebungSekunden.ToInt());
            var ivuZeitpunkt = message.SimulationsZeit.Value.Add(ivuZeit).TimeOfDay;

            var fahrzeuge = message?.Decoder.AsEnumerable();

            var result = new TrainLeg
            {
                EBuEfBetriebsstelleNach = mapping.EBuEfNachBetriebsstelle,
                EBuEfBetriebsstelleVon = mapping.EBuEfVonBetriebsstelle,
                EBuEfGleisNach = message.EndGleis,
                EBuEfGleisVon = message.StartGleis,
                EBuEfZeitpunktNach = ebuefZeitpunktNach,
                EBuEfZeitpunktVon = ebuefZeitpunktVon,
                Fahrzeuge = fahrzeuge,
                IstPrognose = message.Modus == MessageType.Prognose,
                IVUGleis = mapping.IVUGleis,
                IVULegTyp = mapping.IVUTrainPositionType,
                IVUNetzpunkt = mapping.IVUNetzpunkt,
                IVUZeitpunkt = ivuSessionDate.Add(ivuZeitpunkt),
                Zugnummer = message.Zugnummer,
            };

            return result;
        }

        #endregion Private Methods
    }
}