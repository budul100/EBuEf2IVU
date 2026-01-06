using System;
using System.Collections.Generic;
using System.Linq;
using EBuEf2IVU.Services.DatabaseConnector.Models;
using EBuEf2IVU.Shareds.Commons.Models;
using EnumerableExtensions;

namespace EBuEf2IVU.Services.DatabaseConnector.Extensions
{
    internal static class HaltExtensions
    {
        #region Public Fields

        public const char PositiveBit = '1';

        #endregion Public Fields

        #region Public Methods

        public static TimeSpan? GetAbfahrt<T>(this Halt<T> halt)
            where T : Zug
        {
            var result = halt.AbfahrtPlan;

            if (halt is HaltDispo dispoHalt)
            {
                result = dispoHalt.AbfahrtIst ?? dispoHalt.AbfahrtSoll ?? dispoHalt.AbfahrtPlan;
            }

            return result;
        }

        public static DateTime? GetAbfahrtPath<T>(this Halt<T> halt, bool preferPrognosis)
            where T : Zug
        {
            var result = halt.AbfahrtPlan;

            if (halt is HaltDispo dispoHalt)
            {
                result = preferPrognosis
                    ? dispoHalt.AbfahrtPrognose ?? dispoHalt.AbfahrtSoll
                    : dispoHalt.AbfahrtSoll;
            }

            return result.ToDateTime();
        }

        public static DateTime? GetAnkunftPath<T>(this Halt<T> halt, bool preferPrognosis)
            where T : Zug
        {
            var result = halt.AnkunftPlan;

            if (halt is HaltDispo dispoHalt)
            {
                result = preferPrognosis
                    ? dispoHalt.AnkunftPrognose ?? dispoHalt.AnkunftSoll
                    : dispoHalt.AnkunftSoll;
            }

            return result.ToDateTime();
        }

        public static IEnumerable<TrainPosition> GetTrainPositions<T>(this IEnumerable<Halt<T>> halte,
            bool preferPrognosis)
            where T : Zug
        {
            if (halte.AnyItem())
            {
                foreach (var halt in halte)
                {
                    var result = new TrainPosition
                    {
                        Abfahrt = halt.GetAbfahrtPath(preferPrognosis),
                        Ankunft = halt.GetAnkunftPath(preferPrognosis),
                        Bemerkungen = halt.Bemerkungen,
                        Betriebsstelle = halt.Betriebsstelle,
                        Gleis = halt.GleisPlan.ToString(),
                        IstDurchfahrt = halt.IstDurchfahrt,
                        VerkehrNicht = false,
                    };

                    yield return result;
                }
            }
        }

        public static IEnumerable<TrainRun> GetTrainRuns<T>(this IEnumerable<Halt<T>> halte,
            bool preferPrognosis)
            where T : Zug
        {
            if (halte.AnyItem())
            {
                var halteGroups = halte
                    .GroupBy(h => h.ZugID).ToArray();

                foreach (var halteGroup in halteGroups)
                {
                    var ordered = halteGroup
                        .OrderBy(h => h.SortierZeit).ToArray();

                    var positions = ordered.GetTrainPositions(
                        preferPrognosis: preferPrognosis).ToArray();

                    var relevant = ordered[0];

                    var result = new TrainRun
                    {
                        Abfahrt = relevant.GetAbfahrt(),
                        Bemerkungen = relevant.Zug.Bemerkungen,
                        IstGeaendert = false,
                        Positions = positions,
                        Zuggattung = relevant.Zug.Zuggattung.Kurzname,
                        ZugId = relevant.ZugID,
                        Zugnummer = relevant.Zug.Zugnummer,
                    };

                    yield return result;
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static DateTime? ToDateTime(this TimeSpan? time)
        {
            var result = default(DateTime?);

            if (time.HasValue)
            {
                result = new DateTime(time.Value.Ticks);
            }

            return result;
        }

        #endregion Private Methods
    }
}