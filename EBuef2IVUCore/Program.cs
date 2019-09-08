using CommandLine;
using Common.Interfaces;
using Common.Settings;
using DryIoc;
using EBuEf2IVUCore.Internals;
using EBuEf2IVUCore.Services;
using EBuEfDBConnector;
using MessageReceiver;
using RealtimeSender;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.HostConfigurators;
using Topshelf.ServiceConfigurators;

namespace EBuEf2IVUCore
{
    internal class Program
    {
        #region Private Fields

        private const string SettingsFileName = "settings.xml";

        #endregion Private Fields

        #region Private Methods

        private static void ConfigureService(
            HostConfigurator callback, IResolverContext scope, CancellationTokenSource cancellationTokenSource)
        {
            callback.Service<ConversionService>(hostSettings => RunService(
                hostSettings: hostSettings,
                scope: scope,
                cancellationTokenSource: cancellationTokenSource));

            callback.SetDescription(AppInfoService.Description);
            callback.SetDisplayName(AppInfoService.Product);
            callback.SetServiceName(AppInfoService.Product);

            callback.RunAsLocalService();
        }

        private static Container GetContainer(
            EBuEf2IVUSettings settings, CancellationTokenSource cancellationTokenSource)
        {
            var result = new Container();

            result.UseInstance(settings);
            result.UseInstance(cancellationTokenSource.Token);
            result.UseInstance<ILogger>(GetLogger(settings));

            result.Register<IDataManager, DataManager>(Reuse.Singleton);
            result.Register<IReceiverManager, ReceiverManager>(Reuse.Singleton);
            result.Register<ISenderManager, SenderManager>(Reuse.Singleton);

            return result;
        }

        private static Logger GetLogger(EBuEf2IVUSettings settings)
        {
            var levelSwitch = new LoggingLevelSwitch
            {
                MinimumLevel = settings.LogLevel ?? LogEventLevel.Information,
            };

            return new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .WriteTo.Console()
                .WriteTo.File(
                    path: settings.LogFilePath,
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        private static EBuEf2IVUSettings GetSettings(CommandLineOptions options)
        {
            var settingsPath = GetSettingsPath(options);

            var settingsService = new SettingsService<EBuEf2IVUSettings>(settingsPath);
            return settingsService.Settings;
        }

        private static string GetSettingsPath(CommandLineOptions options)
        {
            return !string.IsNullOrWhiteSpace(options.SettingsPath)
                ? options.SettingsPath
                : Path.Combine(
                    path1: Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    path2: SettingsFileName);
        }

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(options => RunWithOptions(options));
        }

        private static void RunService(
            ServiceConfigurator<ConversionService> hostSettings, IResolverContext scope, CancellationTokenSource cancellationTokenSource)
        {
            hostSettings.ConstructUsing(name => new ConversionService());
            hostSettings.WhenStarted(conversionService => Task.Run(() => conversionService.Run(
                receiverManager: scope.Resolve<IReceiverManager>(),
                dataManager: scope.Resolve<IDataManager>(),
                senderManager: scope.Resolve<ISenderManager>(),
                settings: scope.Resolve<EBuEf2IVUSettings>())));
            hostSettings.WhenStopped(conversionService => cancellationTokenSource.Cancel());
        }

        private static void RunWithOptions(CommandLineOptions options)
        {
            var settings = GetSettings(options);
            var cancellationTokenSource = new CancellationTokenSource();
            var container = GetContainer(
                settings: settings,
                cancellationTokenSource: cancellationTokenSource);

            var logger = container.Resolve<ILogger>();

            using (var scope = container.OpenScope())
            {
                if (options.RunStandAlone || !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    logger.Information($"Starte {AppInfoService.ProductTitle} ({AppInfoService.VersionMajorMinor}) im Stand-Alone-Modus.");

                    var conversionService = new ConversionService();
                    Task.Run(() => conversionService.Run(
                        receiverManager: scope.Resolve<IReceiverManager>(),
                        dataManager: scope.Resolve<IDataManager>(),
                        senderManager: scope.Resolve<ISenderManager>(),
                        settings: scope.Resolve<EBuEf2IVUSettings>()));

                    Console.ReadLine();
                }
                else
                {
                    logger.Information($"Starte {AppInfoService.ProductTitle} ({AppInfoService.VersionMajorMinor}) als Windows Service.");

                    var exitCode = HostFactory.Run(callback => ConfigureService(
                        callback: callback,
                        scope: scope,
                        cancellationTokenSource: cancellationTokenSource));
                    var environmentExitCode = (int)Convert.ChangeType(
                        value: exitCode,
                        typeCode: exitCode.GetTypeCode());
                    Environment.ExitCode = environmentExitCode;
                }
            }
        }

        #endregion Private Methods
    }
}