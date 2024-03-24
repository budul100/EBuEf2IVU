using System;
using System.IO;
using System.Threading;
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
                IVUNetzpunkt = "ABC",
                IVUGleis = "9",
                IVULegTyp = Commons.Enums.LegType.Abfahrt,
                Zugnummer = "123"
            };

            sender.Add(trainLeg);

            var cancellationTokenSource = new CancellationTokenSource();

            _ = sender.ExecuteAsync(
                ivuDatum: DateTime.Today,
                sessionStart: DateTime.Now.TimeOfDay,
                cancellationToken: cancellationTokenSource.Token);

            Thread.Sleep(5000);

            Assert.That(hasException, Is.True);
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