using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Runtime.InteropServices;

namespace Common.Extensions
{
    public static class ProgramExtensions
    {
        #region Public Methods

        public static void ConfigureAppConfiguration(this IConfigurationBuilder configBuilder, string path)
        {
            configBuilder.AddXmlFile(
                path: path,
                optional: false,
                reloadOnChange: true);
        }

        public static IHostBuilder GetHostBuilder(this IHostBuilder defaultBuilder)
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? WindowsServiceLifetimeHostBuilderExtensions.UseWindowsService(defaultBuilder)
                : SystemdHostBuilderExtensions.UseSystemd(defaultBuilder);
        }

        public static LoggerConfiguration GetSerilogConfiguration(this HostBuilderContext hostingContext,
            LoggerConfiguration loggerConfiguration)
        {
            var result = loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration)
                .WriteTo.Console(theme: SystemConsoleTheme.Literate);

            return result;
        }

        #endregion Public Methods
    }
}