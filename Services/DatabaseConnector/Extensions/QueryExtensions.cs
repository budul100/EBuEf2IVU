using DatabaseConnector.Models;
using System;

namespace DatabaseConnector.Extensions
{
    internal static class QueryExtensions
    {
        #region Public Methods

        public static TimeSpan? GetAbfahrt(this Halt halt)
        {
            var result = halt.AbfahrtIst ?? halt.AbfahrtSoll ?? halt.AbfahrtPlan;

            return result;
        }

        public static DateTime? GetAbfahrtPath(this Halt halt, bool preferPrognosis)
        {
            var result = preferPrognosis
                ? halt.AbfahrtPrognose ?? halt.AbfahrtSoll
                : halt.AbfahrtSoll;

            return result.ToDateTime();
        }

        public static DateTime? GetAnkunftPath(this Halt halt, bool preferPrognosis)
        {
            var result = preferPrognosis
                ? halt.AnkunftPrognose ?? halt.AnkunftSoll
                : halt.AnkunftSoll;

            return result.ToDateTime();
        }

        public static string GetName(this bool istVon)
        {
            var result = istVon ? "Von" : "Nach";

            return result;
        }

        public static bool HasAbfahrt(this Halt halt)
        {
            return halt.AbfahrtIst.HasValue || halt.AbfahrtSoll.HasValue || halt.AbfahrtPlan.HasValue;
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