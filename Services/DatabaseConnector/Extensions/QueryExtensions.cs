using Common.Enums;
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

        public static TimeSpan? GetAbfahrtPath(this Halt halt, bool preferPrognosis)
        {
            var result = preferPrognosis
                ? halt.AbfahrtPrognose ?? halt.AbfahrtSoll
                : halt.AbfahrtSoll;

            return result;
        }

        public static TimeSpan? GetAnkunftPath(this Halt halt, bool preferPrognosis)
        {
            var result = preferPrognosis
                ? halt.AnkunftPrognose ?? halt.AnkunftSoll
                : halt.AnkunftSoll;

            return result;
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

        public static bool HasRelevantStatus(this Sitzung sitzung)
        {
            var result = sitzung.Status == Convert.ToByte(SessionStates.InPreparation)
                || sitzung.Status == Convert.ToByte(SessionStates.IsRunning)
                || sitzung.Status == Convert.ToByte(SessionStates.IsPaused);

            return result;
        }

        #endregion Public Methods
    }
}