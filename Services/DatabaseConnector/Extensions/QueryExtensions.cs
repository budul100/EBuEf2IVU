using System;

namespace EBuEf2IVU.Services.DatabaseConnector.Extensions
{
    internal static class QueryExtensions
    {
        #region Public Fields

        public const char PositiveBit = '1';

        #endregion Public Fields

        #region Public Methods

        public static string GetName(this bool istVon)
        {
            var result = istVon ? "Von" : "Nach";

            return result;
        }

        public static DayOfWeek GetWochentag(this string bitmask)
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

            return index < 6
                ? (DayOfWeek)(index + 1)
                : DayOfWeek.Sunday;
        }

        #endregion Public Methods
    }
}