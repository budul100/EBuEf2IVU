using Common.Enums;
using Common.Models;
using ConverterExtensions;
using EBuEf2IVUVehicle.Extensions;
using EBuEf2IVUVehicle.Settings;
using EnumerableExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RegexExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EBuEf2IVUVehicle
{
    internal class Message2TrainLeg
    {
        #region Private Fields

        private readonly IEnumerable<InfrastructureMapping> infrastructureMappings;
        private readonly DateTime ivuSessionDate;
        private readonly ILogger logger;

        #endregion Private Fields

        #region Public Constructors

        public Message2TrainLeg(IConfiguration config, ILogger logger, DateTime ivuSessionDate)
        {
            this.logger = logger;
            this.ivuSessionDate = ivuSessionDate;

            infrastructureMappings = config.GetInfrastructureMappings();
        }

        #endregion Public Constructors

        #region Public Methods

        public TrainLeg Convert(RealTimeMessage message)
        {
            var result = default(TrainLeg);

            var mapping = infrastructureMappings
                .Where(m => m.MessageBetriebsstelle.IsMatch(message.Betriebsstelle))
                .Where(m => m.MessageStartGleis.IsMatchOrEmptyPatternOrEmptyValue(message.StartGleis)
                    && m.MessageEndGleis.IsMatchOrEmptyPatternOrEmptyValue(message.EndGleis))
                .OrderByDescending(m => m.MessageStartGleis.IsMatch(message.StartGleis))
                .ThenByDescending(m => m.MessageEndGleis.IsMatch(message.EndGleis)).FirstOrDefault();

            if (mapping != null)
            {
                if ((message.SignalTyp == SignalType.ESig && mapping.IVUTrainPositionType != LegType.Ankunft)
                    || (message.SignalTyp == SignalType.ASig && mapping.IVUTrainPositionType != LegType.Abfahrt)
                    || (message.SignalTyp == SignalType.BkSig && mapping.IVUTrainPositionType != LegType.Durchfahrt))
                {
                    logger.LogWarning(
                        "Der IVUTrainPositionType des Mappings {mappingType} entspricht nicht dem SignalTyp der eingegangenen Nachricht ({message}).",
                        mapping.IVUTrainPositionType,
                        message);
                }

                result = GetTrainLeg(
                    message: message,
                    mapping: mapping);
            }

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private TrainLeg GetTrainLeg(RealTimeMessage message, InfrastructureMapping mapping)
        {
            var ebuefVonZeit = TimeSpan.FromSeconds(mapping.EBuEfVonVerschiebungSekunden.ToInt());
            var ebuefNachZeit = TimeSpan.FromSeconds(mapping.EBuEfNachVerschiebungSekunden.ToInt());

            var ivuZeit = TimeSpan.FromSeconds(mapping.IVUVerschiebungSekunden.ToInt());
            var ivuZeitpunkt = ivuSessionDate.Add(message.SimulationsZeit.Add(ivuZeit).TimeOfDay);

            var fahrzeuge = message?.Decoder.AsEnumerable();

            var result = new TrainLeg
            {
                EBuEfBetriebsstelleNach = mapping.EBuEfNachBetriebsstelle,
                EBuEfBetriebsstelleVon = mapping.EBuEfVonBetriebsstelle,
                EBuEfGleisNach = message.EndGleis,
                EBuEfGleisVon = message.StartGleis,
                EBuEfZeitpunktNach = message.SimulationsZeit.Add(ebuefNachZeit).TimeOfDay,
                EBuEfZeitpunktVon = message.SimulationsZeit.Add(ebuefVonZeit).TimeOfDay,
                Fahrzeuge = fahrzeuge,
                IVUGleis = mapping.IVUGleis,
                IVUNetzpunkt = mapping.IVUNetzpunkt,
                IVULegTyp = mapping.IVUTrainPositionType,
                IVUZeitpunkt = ivuZeitpunkt,
                Zugnummer = message.Zugnummer,
            };

            return result;
        }

        #endregion Private Methods
    }
}