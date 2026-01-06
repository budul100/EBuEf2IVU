using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CommandLine;
using EBuEf2IVU.Shareds.Commons.Enums;
using EBuEf2IVU.Shareds.Commons.Extensions;
using EBuEf2IVU.Shareds.Commons.Interfaces;
using EBuEf2IVU.Shareds.Commons.Settings;
using Serilog;

namespace EBuEf2IVU.Shareds.Host
{
    internal static class Program
    {
        #region Public Methods

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureServices(IServiceCollection services, ServiceType? serviceType)
        {
            services.AddTransient<IMulticastReceiver, Services.MulticastReceiver.Receiver>();
            services.AddTransient<IMQTTReceiver, Services.MQTTReceiver.Receiver>();

            services.AddSingleton<IStateHandler, Services.StateHandler.Handler>();
            services.AddSingleton<IDatabaseConnector, Services.DatabaseConnector.Connector>();

            switch (serviceType)
            {
                case ServiceType.Crew:
                    services.AddSingleton<ICrewChecker, Services.CrewChecker.Checker>();

                    services.AddHostedService<Workers.Crew.Worker>();
                    break;

                case ServiceType.Path:
                    services.AddSingleton<IMessage2TrainRunConverter, Services.Message2TrainRunConverter.Converter>();
                    services.AddSingleton<ITrainPathSender, Services.TrainPathSender.Sender>();

                    services.AddHostedService<Workers.Path.Worker>();
                    break;

                case ServiceType.Vehicle:
                    services.AddSingleton<IMessage2LegConverter, Services.Message2LegConverter.Converter>();
                    services.AddSingleton<IRealtimeSender, Services.RealtimeSender.Sender>();
                    services.AddSingleton<IRealtimeSenderIS, Services.RealtimeSenderIS.Sender>();

                    services.AddHostedService<Workers.Vehicle.Worker>();
                    break;
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var options = default(CommandLineArgs);

            new Parser(config => config.CaseInsensitiveEnumValues = true)
                .ParseArguments<CommandLineArgs>(args)
                .WithParsed(o => options = o);

            var serviceType = options.GetServiceType();
            var settingsPath = options.GetSettingsPath();

            var result = Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices((config) => ConfigureServices(config, serviceType))
                .UseSerilog((hostingContext, loggerConfiguration) => hostingContext.GetSerilogConfiguration(loggerConfiguration));

            return result;
        }

        #endregion Private Methods
    }
}