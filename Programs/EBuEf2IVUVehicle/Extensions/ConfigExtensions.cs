using EBuEf2IVUVehicle.Settings;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace EBuEf2IVUVehicle.Extensions
{
    internal static class ConfigExtensions
    {
        #region Public Methods

        public static IEnumerable<InfrastructureMapping> GetInfrastructureMappings(this IConfiguration configuration)
        {
            var result = configuration
                .GetSection(nameof(InfrastructureMappings))
                .Get<InfrastructureMappings>()
                .InfrastructureMapping;

            return result;
        }

        #endregion Public Methods
    }
}