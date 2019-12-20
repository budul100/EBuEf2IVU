using Common.BusinessObjects;
using Common.Settings;
using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EBuEf2IVUCore.Converters
{
    internal class TrainPositionConverter
    {
        #region Private Fields

        private readonly IEnumerable<InfrastructureMapping> infrastructureMappings;
        private readonly DateTime sessionDateIVU;

        #endregion Private Fields

        #region Public Constructors

        public TrainPositionConverter(IEnumerable<InfrastructureMapping> infrastructureMappings, DateTime sessionDateIVU)
        {
            this.infrastructureMappings = infrastructureMappings;
            this.sessionDateIVU = sessionDateIVU;
        }

        #endregion Public Constructors

        #region Public Methods

        public TrainPosition GetTrainPosition(RealTimeMessage message)
        {
            var result = default(TrainPosition);

            var mapping = infrastructureMappings
                .Where(m => m.MessageBetriebsstelle.IsMatch(message.Betriebsstelle))
                .Where(m => m.MessageStartGleis.IsMatchOrEmptyOrIgnored(message.StartGleis)
                    && m.MessageEndGleis.IsMatchOrEmptyOrIgnored(message.EndGleis))
                .OrderByDescending(m => m.MessageStartGleis.IsMatch(message.StartGleis))
                .ThenByDescending(m => m.MessageEndGleis.IsMatch(message.EndGleis))
                .FirstOrDefault();

            if (mapping != null)
            {
                var ebuefVonZeit = TimeSpan.FromSeconds(mapping.EBuEfVonVerschiebungSekunden.ToInt());
                var ebuefNachZeit = TimeSpan.FromSeconds(mapping.EBuEfNachVerschiebungSekunden.ToInt());
                var ivuZeit = TimeSpan.FromSeconds(mapping.IVUVerschiebungSekunden.ToInt());

                var ivuZeitpunkt = sessionDateIVU.Add(message.SimulationsZeit.Add(ivuZeit).TimeOfDay);

                result = new TrainPosition
                {
                    EBuEfBetriebsstelleNach = mapping.EBuEfNachBetriebsstelle,
                    EBuEfBetriebsstelleVon = mapping.EBuEfVonBetriebsstelle,
                    EBuEfGleisNach = message.EndGleis,
                    EBuEfGleisVon = message.StartGleis,
                    EBuEfZeitpunktNach = message.SimulationsZeit.Add(ebuefNachZeit).TimeOfDay,
                    EBuEfZeitpunktVon = message.SimulationsZeit.Add(ebuefVonZeit).TimeOfDay,
                    Fahrzeuge = GetFahrzeuge(message).ToArray(),
                    IVUGleis = mapping.IVUGleis,
                    IVUNetzpunkt = mapping.IVUNetzpunkt,
                    IVUTrainPositionTyp = mapping.IVUTrainPositionType,
                    IVUZeitpunkt = ivuZeitpunkt,
                    Zugnummer = message.Zugnummer,
                };
            }

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private IEnumerable<string> GetFahrzeuge(RealTimeMessage message)
        {
            yield return message.Decoder;
        }

        #endregion Private Methods
    }
}