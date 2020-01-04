using CommandLine;
using EBuEf2IVUCore.Models;
using EBuEf2IVUCore.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace EBuEf2IVUCore
{
    public static class Program
    {
        #region Private Fields

        private const string LogFileName = "ebuef2ivu.log";
        private const string SettingsFileName = "settings.xml";

        #endregion Private Fields

        #region Public Methods

        public static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<CommandLineOptions>(args)
                .WithParsed(options => RunWorker(
                    args: args,
                    options: options));
        }

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureAppConfiguration(IConfigurationBuilder configBuilder, CommandLineOptions options)
        {
            configBuilder.AddXmlFile(
                path: GetSettingsPath(options),
                optional: false,
                reloadOnChange: true);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<Worker>();
        }

        private static IHostBuilder GetHostBuilder(this IHostBuilder defaultBuilder)
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? SystemdHostBuilderExtensions.UseSystemd(defaultBuilder)
                : WindowsServiceLifetimeHostBuilderExtensions.UseWindowsService(defaultBuilder);
        }

        private static string GetLogFilePath(IConfiguration config)
        {
            var settings = config
                .GetSection(nameof(Logging))
                .Get<Logging>();

            return !string.IsNullOrWhiteSpace(settings.LogFilePath)
                ? settings.LogFilePath.Trim()
                : Path.Combine(
                    path1: Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    path2: LogFileName);
        }

        private static LoggerConfiguration GetLoggerConfiguration(HostBuilderContext hostingContext,
            LoggerConfiguration loggerConfiguration)
        {
            var result = loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration)
                .WriteTo.Console(
                    theme: ConsoleTheme.None)
                .WriteTo.File(
                    path: GetLogFilePath(hostingContext.Configuration),
                    rollingInterval: RollingInterval.Day);

            return result;
        }

        private static string GetSettingsPath(CommandLineOptions options)
        {
            var result = !string.IsNullOrWhiteSpace(options.SettingsPath)
                ? options.SettingsPath
                : Path.Combine(
                    path1: Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    path2: SettingsFileName);

            return result;
        }

        private static void RunWorker(string[] args, CommandLineOptions options)
        {
            var hostBuilder = Host
                .CreateDefaultBuilder(args)
                .GetHostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) => ConfigureAppConfiguration(
                    configBuilder: config,
                    options: options))
                .ConfigureServices((hostContext, services) => ConfigureServices(services))
                .UseSerilog((hostingContext, loggerConfiguration) => GetLoggerConfiguration(hostingContext, loggerConfiguration));

            hostBuilder.Build().Run();
        }

        #endregion Private Methods
    }
}