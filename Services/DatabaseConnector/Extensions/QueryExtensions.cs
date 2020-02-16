using DatabaseConnector.Models;
using System;

namespace DatabaseConnector.Extensions
{
    internal static class QueryExtensions
    {
        #region Public Methods

        public static bool IsInMaxTime(this Halt halt, TimeSpan maxTime)
        {
            if (halt.AbfahrtSoll.HasValue)
            {
                return halt.AbfahrtSoll <= maxTime;
            }
            else if (halt.AbfahrtPlan.HasValue)
            {
                return halt.AbfahrtPlan <= maxTime;
            }

            return false;
        }

        public static bool IsInMinTime(this Halt halt, TimeSpan minTime)
        {
            if (halt.AbfahrtIst.HasValue)
            {
                return halt.AbfahrtIst >= minTime;
            }

            return halt.AbfahrtPlan.HasValue || halt.AbfahrtSoll.HasValue;
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