using CommandLine;
using Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace EBuEf2IVUVehicle
{
    public static class Program
    {
        #region Private Fields

        private const string SettingsFileName = "settings.xml";

        #endregion Private Fields

        #region Public Methods

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var result = default(IHostBuilder);

            Parser.Default
                .ParseArguments<Settings.CommandLine>(args)
                .WithParsed(options =>
                {
                    result = Host
                    .CreateDefaultBuilder(args)
                    .GetHostBuilder()
                    .ConfigureAppConfiguration((hostingContext, config) => ConfigureAppConfiguration(
                        configBuilder: config,
                        options: options))
                    .ConfigureServices((hostContext, services) => ConfigureServices(services))
                    .UseSerilog((hostingContext, loggerConfiguration) => GetSerilogConfiguration(
                        hostingContext: hostingContext,
                        loggerConfiguration: loggerConfiguration));
                });

            return result;
        }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureAppConfiguration(IConfigurationBuilder configBuilder, Settings.CommandLine options)
        {
            configBuilder.AddXmlFile(
                path: GetSettingsPath(options),
                optional: false,
                reloadOnChange: true);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<Worker>();

            services.AddSingleton<IConnector, DatabaseConnector.Connector>();
            services.AddSingleton<IStateHandler, StateHandler.Handler>();
        }

        private static IHostBuilder GetHostBuilder(this IHostBuilder defaultBuilder)
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? WindowsServiceLifetimeHostBuilderExtensions.UseWindowsService(defaultBuilder)
                : SystemdHostBuilderExtensions.UseSystemd(defaultBuilder);
        }

        private static LoggerConfiguration GetSerilogConfiguration(HostBuilderContext hostingContext,
            LoggerConfiguration loggerConfiguration)
        {
            var result = loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration)
                .WriteTo.Console(theme: SystemConsoleTheme.Literate);

            return result;
        }

        private static string GetSettingsPath(Settings.CommandLine options)
        {
            var result = !string.IsNullOrWhiteSpace(options?.SettingsPath)
                ? options.SettingsPath
                : Path.Combine(
                    path1: Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    path2: SettingsFileName);

            return result;
        }

        #endregion Private Methods
    }
}