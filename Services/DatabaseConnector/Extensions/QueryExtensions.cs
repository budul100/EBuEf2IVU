using DatabaseConnector.Models;
using System;

namespace DatabaseConnector.Extensions
{
    internal static class QueryExtensions
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
                result = dispoHalt.AbfahrtIst ?? dispoHalt.AbfahrtSoll ?? halt.AbfahrtPlan;
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

        public static string GetName(this bool istVon)
        {
            var result = istVon ? "Von" : "Nach";

            return result;
        }

        public static int GetWochentagId(this string bitmask)
        {
            var bits = bitmask.ToCharArray();

            var index = 0;
            foreach (var bit in bits)
            {
                if (bit == PositiveBit)
                {
                    break;
                }

                index++;
            }

            return index;
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