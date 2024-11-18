using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Commons.Extensions;
using Commons.Interfaces;
using Commons.Models;
using EBuEf2IVUTestBase;
using Moq;
using NUnit.Framework;
using RealtimeSender;

namespace RealTimeSenderTests
{
    public class Tests
        : TestsBase
    {
        #region Private Fields

        // Set this value as true if you have an active IVU instance running.
        private const bool IVUIsRunning = true;

        private const string SettingsPath = @"..\..\..\RealTimeSenderTests.example.xml";

        #endregion Private Fields

        #region Public Methods

        [Test]
        public void SendRealtime()
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

            var cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => sender.ExecuteAsync(
                ivuDatum: DateTime.Today,
                sessionStart: DateTime.Now.TimeOfDay,
                cancellationToken: cancellationTokenSource.Token));

            sender.Add(trainLeg);

            Thread.Sleep(2000);

            Assert.That(hasException, IVUIsRunning ? Is.False : Is.True);
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