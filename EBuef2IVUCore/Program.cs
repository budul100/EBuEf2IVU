using CommandLine;
using EBuEf2IVUCore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace EBuEf2IVUCore
{
    public class Program
    {
        #region Private Fields

        private const string SettingsFileName = "settings.xml";

        #endregion Private Fields

        #region Public Methods

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(options => RunWorker(
                    args: args,
                    options: options));
        }

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureAppConfiguration(IConfigurationBuilder config, CommandLineOptions options)
        {
            config.AddXmlFile(
                path: GetSettingsPath(options),
                optional: false,
                reloadOnChange: true);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<Worker>();
        }

        private static IHostBuilder GetHostBuilderLinux(string[] args, CommandLineOptions options)
        {
            var result = Host
                .CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureAppConfiguration((hostingContext, config) => ConfigureAppConfiguration(
                    config: config,
                    options: options))
                .ConfigureServices((hostContext, services) => ConfigureServices(services));

            return result;
        }

        private static IHostBuilder GetHostBuilderWindows(string[] args, CommandLineOptions options)
        {
            var result = Host
                .CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureAppConfiguration((hostingContext, config) => ConfigureAppConfiguration(
                    config: config,
                    options: options))
                .ConfigureServices((hostContext, services) => ConfigureServices(services));

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
            var hostBuilder = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? GetHostBuilderWindows(
                    args: args,
                    options: options)
                : GetHostBuilderLinux(
                    args: args,
                    options: options);

            hostBuilder.Build().Run();
        }

        #endregion Private Methods
    }
}