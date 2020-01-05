using System;

namespace DatabaseConnector.Extensions
{
    internal static class DateTimeExtensions
    {
        #region Public Methods

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