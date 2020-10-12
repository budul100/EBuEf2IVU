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
            return halt.AbfahrtIst ?? halt.AbfahrtSoll ?? halt.AbfahrtPlan;
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