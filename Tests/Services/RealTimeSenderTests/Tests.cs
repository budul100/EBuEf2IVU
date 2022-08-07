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
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeSenderTests
{
    public class Tests
        : TestsBase
    {
        private const string SettingsPath = @"..\..\..\..\..\..\Programs\EBuEf2IVUVehicle\ebuef2ivuvehicle-settings.example.xml";

        #region Public Methods

        [Test]
        public void SendRealtime()
        {
            var hasSocketException = false;

            var loggerMock = GetLoggerMock(() => hasSocketException = true);

            var settingsPath = Path.GetFullPath(SettingsPath);

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices(services => ConfigureServices(services, loggerMock))
                .Build();

            var senderSettings = host.Services.GetService<IConfiguration>()
                .GetSection(nameof(EBuEf2IVUVehicle.Settings.RealtimeSender))
                .Get<EBuEf2IVUVehicle.Settings.RealtimeSender>();

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
                IVUZeitpunkt = DateTime.Now,
                IVUGleis = "9",
                IVULegTyp = Common.Enums.LegType.Abfahrt,
                Zugnummer = "123"
            };

            sender.Add(trainLeg);

            var cancellationTokenSource = new CancellationTokenSource();

            Task.WhenAny(
                _ = sender.ExecuteAsync(cancellationTokenSource.Token));

            Thread.Sleep(6000);

            Assert.True(hasSocketException);
        }

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureServices(IServiceCollection services, Mock<ILogger<Sender>> loggerMock)
        {
            services.AddSingleton(loggerMock.Object);
            services.AddSingleton<IRealtimeSender, Sender>();
        }

        private static Mock<ILogger<Sender>> GetLoggerMock(Action errorCallback)
        {
            var result = new Mock<ILogger<Sender>>();

            result
                .Setup(x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<SocketException>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()))
                .Callback(() => errorCallback?.Invoke());

            return result;
        }

        #endregion Private Methods
    }
}