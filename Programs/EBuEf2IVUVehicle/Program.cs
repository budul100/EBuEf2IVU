﻿using CommandLine;
using Common.Extensions;
using Common.Interfaces;
using EBuEf2IVUBase.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using StringExtensions;
using System.IO;
using System.Reflection;

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
                .WithParsed(options =>
                {
                    result = Host
                        .CreateDefaultBuilder(args)
                        .GetHostBuilder()
                        .ConfigureAppConfiguration((hostingContext, config) => config.ConfigureAppConfiguration(GetSettingsPath(options)))
                        .ConfigureServices((hostContext, services) => ConfigureServices(services))
                        .UseSerilog((hostingContext, loggerConfiguration) => hostingContext.GetSerilogConfiguration(loggerConfiguration));
                });

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
        }

        private static string GetSettingsPath(CommandLineArgs args)
        {
            var result = args.SettingsPath;

            if (result.IsEmpty())
            {
                result = Path.Combine(
                    path1: Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    path2: SettingsFileName);
            }

            return result;
        }

        #endregion Private Methods
    }
}