﻿using CommandLine;
using Commons.Extensions;
using Commons.Interfaces;
using EBuEf2IVUBase.Extensions;
using EBuEf2IVUBase.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EBuEf2IVUCrew
{
    public static class Program
    {
        #region Private Fields

        private const string SettingsFileName = "ebuef2ivucrew-settings.xml";

        #endregion Private Fields

        #region Public Methods

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<Worker>();

            services.AddSingleton<IDatabaseConnector, DatabaseConnector.Connector>();

            services.AddTransient<IMulticastReceiver, MulticastReceiver.Receiver>();
            services.AddTransient<IMQTTReceiver, MQTTReceiver.Receiver>();

            services.AddSingleton<IStateHandler, StateHandler.Handler>();
            services.AddSingleton<ICrewChecker, CrewChecker.Checker>();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var result = default(IHostBuilder);

            Parser.Default
                .ParseArguments<CommandLineArgs>(args)
                .WithParsed(options => result = GetHostBuilder(
                    options: options,
                    args: args));

            return result;
        }

        private static IHostBuilder GetHostBuilder(CommandLineArgs options, string[] args)
        {
            var settingsPath = options.GetSettingsPath(SettingsFileName);

            var result = Host
                .CreateDefaultBuilder(args)
                .GetHostBuilder()
                .ConfigureAppConfiguration(config => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices(services => ConfigureServices(services))
                .UseSerilog((hostingContext, loggerConfiguration) => hostingContext.GetSerilogConfiguration(loggerConfiguration));

            return result;
        }

        #endregion Private Methods
    }
}