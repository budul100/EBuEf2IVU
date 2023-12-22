using Commons.Settings;
using System;

namespace Commons.Extensions
{
    public static class SettingsExtensions
    {
        #region Public Fields

        public const string EnvironmentIVUEndpoint = "IVU_IFSERVER_ENDPOINT";
        public const string EnvironmentIVUHost = "IVU_APPSERVER_HOST";
        public const string EnvironmentIVUIsHttps = "IVU_APPSERVER_ISHTTPS";
        public const string EnvironmentIVUPort = "IVU_APPSERVER_PORT";

        #endregion Public Fields

        #region Public Methods

        public static string GetEndpoint(this RealtimeSender settings)
        {
            var result = settings?.Endpoint;

            if (string.IsNullOrWhiteSpace(result))
            {
                result = Environment.GetEnvironmentVariable(EnvironmentIVUEndpoint);
            }

            return result;
        }

        public static string GetHost<T>(this T settings)
            where T : ConnectorIVUBase
        {
            var result = settings?.Host;

            if (string.IsNullOrWhiteSpace(result))
            {
                result = Environment.GetEnvironmentVariable(EnvironmentIVUHost);
            }

            return result;
        }

        public static bool? GetIsHttps<T>(this T settings)
            where T : ConnectorIVUBase
        {
            var result = settings?.IsHttps;

            if (!result.HasValue
                && bool.TryParse(
                    value: Environment.GetEnvironmentVariable(EnvironmentIVUIsHttps),
                    result: out bool value))
            {
                result = value;
            }

            return result;
        }

        public static int? GetPort<T>(this T settings)
            where T : ConnectorIVUBase
        {
            var result = settings?.Port;

            if (!result.HasValue
                && Int32.TryParse(
                    s: Environment.GetEnvironmentVariable(EnvironmentIVUPort),
                    result: out int value))
            {
                result = value;
            }

            return result;
        }

        #endregion Public Methods
    }
}