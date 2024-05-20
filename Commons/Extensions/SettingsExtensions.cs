using Commons.Settings;
using System;

namespace Commons.Extensions
{
    public static class SettingsExtensions
    {
        #region Public Fields

        public const string EnvironmentDBHost = "MYSQL_STD_HOST";
        public const string EnvironmentDBName = "MYSQL_STD_DBNAME";
        public const string EnvironmentDBPassword = "MYSQL_STD_PASSWORD";
        public const string EnvironmentDBUser = "MYSQL_STD_USER";

        public const string EnvironmentEBuEfFormat = "MESSAGE_FORMAT";
        public const string EnvironmentEBuEfFormatMulticast = "multicast";
        public const string EnvironmentEBuEfHostMC = "EBUEF_HOSTNAME";
        public const string EnvironmentEBuEfHostMQTT = "MQTT_BROKER_IP";
        public const string EnvironmentEBuEfPort = "EBUEF_HOSTPORT";

        public const string EnvironmentIVUAppHost = "IVU_APPSERVER_HOST";
        public const string EnvironmentIVUAppPort = "IVU_APPSERVER_PORT";
        public const string EnvironmentIVUAppSecure = "IVU_APPSERVER_ISHTTPS";
        public const string EnvironmentIVUIFEndpoint = "IVU_IFSERVER_ENDPOINT";

        #endregion Public Fields

        #region Public Methods

        public static string GetDBConnectionString(this EBuEfDBConnector settings)
        {
            var result = settings?.ConnectionString;

            if (string.IsNullOrWhiteSpace(result))
            {
                var server = Environment.GetEnvironmentVariable(EnvironmentDBHost);
                var database = Environment.GetEnvironmentVariable(EnvironmentDBName);
                var uid = Environment.GetEnvironmentVariable(EnvironmentDBUser);
                var password = Environment.GetEnvironmentVariable(EnvironmentDBPassword);

                result = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};SslMode=none";
            }

            return result;
        }

        public static string GetEBuEfHostMC<T>(this T settings)
            where T : ConnectorEBuEfBase
        {
            var result = settings?.Host;

            if (string.IsNullOrWhiteSpace(result))
            {
                result = Environment.GetEnvironmentVariable(EnvironmentEBuEfHostMC);
            }

            return result;
        }

        public static string GetEBuEfHostMQTT<T>(this T settings)
            where T : ConnectorEBuEfBase
        {
            var result = settings?.Host;

            if (string.IsNullOrWhiteSpace(result))
            {
                result = Environment.GetEnvironmentVariable(EnvironmentEBuEfHostMQTT);
            }

            return result;
        }

        public static int? GetEBuEfPort<T>(this T settings)
            where T : ConnectorEBuEfBase
        {
            var result = settings?.Port;

            if (!result.HasValue
                && Int32.TryParse(
                    s: Environment.GetEnvironmentVariable(EnvironmentEBuEfPort),
                    result: out int value))
            {
                result = value;
            }

            return result;
        }

        public static bool GetEBuEfUseMC<T>(this T settings)
            where T : ConnectorEBuEfBase
        {
            var result = settings?.UseMulticast;

            if (!result.HasValue)
            {
                result = Environment.GetEnvironmentVariable(EnvironmentEBuEfFormat) ==
                    Environment.GetEnvironmentVariable(EnvironmentEBuEfFormatMulticast);
            }

            return result ?? false;
        }

        public static string GetIVUAppServerHost<T>(this T settings)
            where T : ConnectorIVUBase
        {
            var result = settings?.Host;

            if (string.IsNullOrWhiteSpace(result))
            {
                result = Environment.GetEnvironmentVariable(EnvironmentIVUAppHost);
            }

            return result;
        }

        public static int? GetIVUAppServerPort<T>(this T settings)
            where T : ConnectorIVUBase
        {
            var result = settings?.Port;

            if (!result.HasValue
                && Int32.TryParse(
                    s: Environment.GetEnvironmentVariable(EnvironmentIVUAppPort),
                    result: out int value))
            {
                result = value;
            }

            return result;
        }

        public static bool? GetIVUAppServerSecure<T>(this T settings)
            where T : ConnectorIVUBase
        {
            var result = settings?.IsHttps;

            if (!result.HasValue
                && bool.TryParse(
                    value: Environment.GetEnvironmentVariable(EnvironmentIVUAppSecure),
                    result: out bool value))
            {
                result = value;
            }

            return result;
        }

        public static string GetIVUIFServerEndpoint(this RealtimeSender settings)
        {
            var result = settings?.Endpoint;

            if (string.IsNullOrWhiteSpace(result))
            {
                result = Environment.GetEnvironmentVariable(EnvironmentIVUIFEndpoint);
            }

            return result;
        }

        #endregion Public Methods
    }
}