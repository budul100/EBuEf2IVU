using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using EBuEf2IVU.Services.Message2LegConverter.Settings;
using EBuEf2IVU.Shareds.Commons.Settings;

namespace EBuEf2IVU.Services.Message2LegConverter.Extensions
{
    internal static class ConfigExtensions
    {
        #region Public Methods

        public static DateTime? GetDateMin(this IConfiguration configuration)
        {
            var result = configuration
                .GetSection(nameof(RealtimeSender))
                .Get<RealtimeSender>().DateMin;

            return result;
        }

        public static IEnumerable<InfrastructureMapping> GetInfrastructureMappings(this IConfiguration configuration)
        {
            var result = configuration
                .GetSection(nameof(InfrastructureMappings))
                .Get<InfrastructureMappings>().InfrastructureMapping;

            return result;
        }

        #endregion Public Methods
    }
}