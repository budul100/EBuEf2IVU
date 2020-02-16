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

        public static DateTime ToDateTime(this long unixTimeStamp)
        {
            var result = new DateTime(
                year: 1970,
                month: 1,
                day: 1,
                hour: 0,
                minute: 0,
                second: 0,
                millisecond: 0,
                kind: DateTimeKind.Utc);
            result = result.AddMilliseconds(unixTimeStamp);

            return result;
        }

        #endregion Public Methods
    }
}