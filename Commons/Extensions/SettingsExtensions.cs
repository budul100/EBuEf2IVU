using System;
using Commons.Settings;
using StringExtensions;

namespace Commons.Extensions
{
    public static class SettingsExtensions
    {
        #region Public Methods

        public static string GetDBConnectionString(this EBuEfDBConnector settings)
        {
            var result = default(string);

            if (!Environment.GetEnvironmentVariable(EBuEfDBConnector.EnvironmentDBHost).IsEmpty()
                && !Environment.GetEnvironmentVariable(EBuEfDBConnector.EnvironmentDBName).IsEmpty()
                && !Environment.GetEnvironmentVariable(EBuEfDBConnector.EnvironmentDBUser).IsEmpty()
                && !Environment.GetEnvironmentVariable(EBuEfDBConnector.EnvironmentDBPassword).IsEmpty())
            {
                var server = Environment.GetEnvironmentVariable(EBuEfDBConnector.EnvironmentDBHost);
                var database = Environment.GetEnvironmentVariable(EBuEfDBConnector.EnvironmentDBName);
                var uid = Environment.GetEnvironmentVariable(EBuEfDBConnector.EnvironmentDBUser);
                var password = Environment.GetEnvironmentVariable(EBuEfDBConnector.EnvironmentDBPassword);

                result = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};SslMode=none";
            }

            if (result.IsEmpty())
            {
                result = settings?.ConnectionString;
            }

            return result;
        }

        public static string GetIVUAppServerHost<T>(this T settings)
            where T : ConnectorIVUBase
        {
            var result = Environment.GetEnvironmentVariable(ConnectorIVUBase.EnvironmentHost);

            if (result.IsEmpty())
            {
                result = settings?.Host;
            }

            return result;
        }

        public static int GetIVUAppServerPort<T>(this T settings)
            where T : ConnectorIVUBase
        {
            var result = default(int?);

            if (int.TryParse(
                s: Environment.GetEnvironmentVariable(ConnectorIVUBase.EnvironmentPort),
                result: out int value))
            {
                result = value;
            }

            if (!result.HasValue)
            {
                result = settings?.Port;
            }

            return result ?? ConnectorIVUBase.IVUAppServerPortDefault;
        }

        public static string GetIVUIFServerEndpoint(this RealtimeSender settings)
        {
            var result = Environment.GetEnvironmentVariable(RealtimeSender.EnvironmentIVUIFEndpoint);

            if (result.IsEmpty())
            {
                result = settings?.Endpoint;
            }

            return result;
        }

        public static string GetMCHost<T>(this T settings, string variableName)
            where T : ConnectorEBuEfBase
        {
            var result = Environment.GetEnvironmentVariable(variableName);

            if (result.IsEmpty())
            {
                result = settings?.Host;
            }

            return result;
        }

        public static int GetMCPort<T>(this T settings, string variableName, int defaultPort)
            where T : ConnectorEBuEfBase
        {
            var result = default(int?);

            if (int.TryParse(
                s: Environment.GetEnvironmentVariable(variableName),
                result: out int value))
            {
                result = value;
            }

            if (!result.HasValue)
            {
                result = settings?.Port;
            }

            return result ?? defaultPort;
        }

        public static string GetMQTTHost<T>(this T settings, string variableName)
            where T : ConnectorEBuEfBase
        {
            var result = Environment.GetEnvironmentVariable(variableName);

            if (result.IsEmpty())
            {
                result = settings?.Host;
            }

            return result;
        }

        public static int? GetMQTTPort<T>(this T settings, string variableName)
            where T : ConnectorEBuEfBase
        {
            var result = default(int?);

            if (int.TryParse(
                s: Environment.GetEnvironmentVariable(variableName),
                result: out int value))
            {
                result = value;
            }

            if (!result.HasValue)
            {
                result = settings?.Port;
            }

            return result;
        }

        public static string GetMQTTTopic<T>(this T settings, string variableName)
            where T : ConnectorEBuEfBase
        {
            var result = Environment.GetEnvironmentVariable(variableName);

            if (result.IsEmpty())
            {
                result = settings?.Topic;
            }

            return result;
        }

        public static bool IsIVUAppServerHttps<T>(this T settings)
            where T : ConnectorIVUBase
        {
            var result = default(bool?);

            if (bool.TryParse(
                value: Environment.GetEnvironmentVariable(ConnectorIVUBase.EnvironmentIsHttps),
                result: out bool value))
            {
                result = value;
            }

            if (!result.HasValue)
            {
                result = settings?.IsHttps;
            }

            return result ?? false;
        }

        public static bool UseMulticast<T>(this T settings, string variableName)
            where T : ConnectorEBuEfBase
        {
            var result = default(bool?);

            if (!Environment.GetEnvironmentVariable(variableName).IsEmpty())
            {
                result = Environment.GetEnvironmentVariable(variableName) ==
                    ConnectorEBuEfBase.FormatMulticast;
            }

            if (!result.HasValue)
            {
                result = settings?.UseMulticast;
            }

            return result ?? false;
        }

        #endregion Public Methods
    }
}