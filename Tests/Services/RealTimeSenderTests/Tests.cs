using Common.Extensions;
using Common.Interfaces;
using Common.Models;
using EBuEf2IVUTestBase;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RealtimeSender;
using System;
using System.IO;
using System.Threading;

namespace RealTimeSenderTests
{
    public class Tests
        : TestsBase
    {
        #region Private Fields

        private const string SettingsPath = @"..\..\..\ebuef2ivuvehicle-settings.example.xml";

        #endregion Private Fields

        #region Public Methods

        [Test]
        public void SendRealtime()
        {
            var hasSocketException = false;

            var loggerMock = GetLoggerMock<Sender>(() => hasSocketException = true);

            var settingsPath = Path.GetFullPath(SettingsPath);

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices(services => ConfigureServices(services, loggerMock))
                .Build();

            var senderSettings = host.Services.GetService<IConfiguration>()
                .GetSection(nameof(Common.Settings.RealtimeSender))
                .Get<Common.Settings.RealtimeSender>();

            var sender = host.Services.GetService<IRealtimeSender>();

            sender.Initialize(
                host: senderSettings.Host,
                port: senderSettings.Port,
                path: senderSettings.Path,
                username: senderSettings.Username,
                password: senderSettings.Password,
                isHttps: senderSettings.IsHttps,
                division: senderSettings.Division,
                retryTime: senderSettings.RetryTime);

            var trainLeg = new TrainLeg
            {
                IVUNetzpunkt = "ABC",
                IVUGleis = "9",
                IVULegTyp = Common.Enums.LegType.Abfahrt,
                Zugnummer = "123"
            };

            sender.Add(trainLeg);

            var cancellationTokenSource = new CancellationTokenSource();

            _ = sender.ExecuteAsync(
                ivuDatum: DateTime.Today,
                sessionStart: DateTime.Now.TimeOfDay,
                cancellationToken: cancellationTokenSource.Token);

            Thread.Sleep(5000);

            Assert.True(hasSocketException);
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