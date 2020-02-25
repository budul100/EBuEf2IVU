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

        #endregion Public Methods
    }
}