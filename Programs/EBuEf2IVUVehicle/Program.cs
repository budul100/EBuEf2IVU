using CommandLine;
using Common.Extensions;
using Common.Interfaces;
using EBuEf2IVUBase.Extensions;
using EBuEf2IVUBase.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EBuEf2IVUVehicle
{
    public static class Program
    {
        #region Private Fields

        private const string SettingsFileName = "ebuef2ivuvehicle-settings.xml";

        #endregion Private Fields

        #region Public Methods

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var result = default(IHostBuilder);

            Parser.Default
                .ParseArguments<CommandLineArgs>(args)
                .WithParsed(options => result = GetHostBuilder(
                    options: options,
                    args: args));

            return result;
        }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<Worker>();

            services.AddTransient<IMessageReceiver, MessageReceiver.Receiver>();
            services.AddSingleton<IStateHandler, StateHandler.Handler>();
            services.AddSingleton<IDatabaseConnector, DatabaseConnector.Connector>();

            services.AddSingleton<IRealtimeSender, RealtimeSender.Sender>();
            services.AddSingleton<IRealtimeSenderIS, RealtimeSenderIS.Sender>();
        }

        private static IHostBuilder GetHostBuilder(CommandLineArgs options, string[] args)
        {
            var settingsPath = options.GetSettingsPath(SettingsFileName);

            var result = Host
                .CreateDefaultBuilder(args)
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices((_, services) => ConfigureServices(services))
                .UseSerilog((hostingContext, loggerConfiguration) => hostingContext.GetSerilogConfiguration(loggerConfiguration));

            return result;
        }

        #endregion Private Methods
    }
}