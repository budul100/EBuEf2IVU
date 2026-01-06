using System;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EBuEf2IVU.Services.RealtimeSender;
using EBuEf2IVU.Shareds.Commons.Extensions;
using EBuEf2IVU.Shareds.Commons.Interfaces;
using EBuEf2IVU.Shareds.Commons.Models;
using EBuEf2IVU.Shareds.TestsBase;
using Moq;

namespace EBuEf2IVU.Shareds.RealTimeSenderTests
{
    public class Tests
        : Base
    {
        #region Private Fields

        private const string SettingsPath = @"..\..\..\Tests.xml";

        #endregion Private Fields

        #region Public Methods

        [Fact]
        public async Task SendRealtime()
        {
            var hasException = false;

            var loggerMock = GetLoggerMock<Sender>(() => hasException = true);

            var settingsPath = Path.GetFullPath(SettingsPath);

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices(services => ConfigureServices(services, loggerMock))
                .Build();

            var senderSettings = host.Services.GetService<IConfiguration>()
                .GetSection(nameof(Commons.Settings.RealtimeSender))
                .Get<Commons.Settings.RealtimeSender>();

            var sender = host.Services.GetService<IRealtimeSender>();

            sender.Initialize(
                host: senderSettings.Host,
                port: senderSettings.Port ?? 0,
                isHttps: senderSettings.IsHttps ?? true,
                username: senderSettings.Username,
                password: senderSettings.Password,
                path: senderSettings.Path,
                division: senderSettings.Division,
                retryTime: senderSettings.RetryTime);

            var trainLeg = new TrainLeg
            {
                IVUNetzpunkt = "XWF",
                IVUGleis = "9",
                IVULegTyp = Commons.Enums.LegType.Abfahrt,
                Zugnummer = "123"
            };

            using var cancellationTokenSource = new CancellationTokenSource();

            var task = Task.Run(() => sender.ExecuteAsync(
                ivuDatum: DateTime.Today,
                sessionStart: DateTime.Now.TimeOfDay,
                cancellationToken: cancellationTokenSource.Token));

            // Kurze Verzögerung, damit ExecuteAsync starten kann
            await Task.Delay(100, CancellationToken.None);

            sender.Add(trainLeg);

            // Wartezeit für Verarbeitung
            await Task.Delay(1000, CancellationToken.None);

            var hasCommunicationException = false;

            try
            {
                await task.WaitAsync(
                    TimeSpan.FromSeconds(5),
                    CancellationToken.None);
            }
            catch (CommunicationException)
            {
                hasCommunicationException = true;
            }
            finally
            {
                cancellationTokenSource.Cancel();
            }

            Assert.True(hasCommunicationException || hasException);
        }

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureServices(IServiceCollection services, Mock<ILogger<Sender>> loggerMock)
        {
            services.AddSingleton(loggerMock.Object);
            services.AddSingleton<IRealtimeSender, Sender>();
        }

        #endregion Private Methods
    }
}