using Common.Extensions;
using Common.Models;
using EBuEf2IVUTestBase;
using Message2LegConverter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.IO;

namespace Message2LegConverterTests
{
    public class Tests
        : TestsBase
    {
        #region Private Fields

        private const string SettingsPath = @"..\..\..\..\..\..\Programs\EBuEf2IVUVehicle\ebuef2ivuvehicle-settings.example.xml";

        #endregion Private Fields

        #region Public Methods

        [Test]
        public void ConvertMessage()
        {
            var settingsPath = Path.GetFullPath(SettingsPath);

            var host = Host
                .CreateDefaultBuilder()
                .GetHostBuilder()
                .ConfigureAppConfiguration((_, config) => config.ConfigureAppConfiguration(settingsPath))
                .ConfigureServices(services => ConfigureServices(services))
                .Build();

            var converter = host.Services.GetService<IMessage2LegConverter>();

            var time1 = DateTime.Now;

            converter.Initialize(time1.Date);

            var message1 = new RealTimeMessage
            {
                SimulationsZeit = new DateTime(time1.TimeOfDay.Ticks),
            };

            var result1 = converter.Convert(message1);

            Assert.True(result1.IVUZeitpunkt == time1);

            var time2 = DateTime.Now.AddDays(1);

            converter.Initialize(time2.Date);

            var message2 = new RealTimeMessage
            {
                SimulationsZeit = new DateTime(time2.TimeOfDay.Ticks),
            };

            var result2 = converter.Convert(message2);

            Assert.True(result2.IVUZeitpunkt == time2);

            var time3 = DateTime.Now.AddDays(-1);

            converter.Initialize(time3.Date);

            var message3 = new RealTimeMessage
            {
                SimulationsZeit = new DateTime(time3.TimeOfDay.Ticks),
            };

            var result3 = converter.Convert(message3);

            Assert.True(result3.IVUZeitpunkt == time3);
        }

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<Mock<ILogger<Converter>>>();
            services.AddSingleton<IMessage2LegConverter, Converter>();
        }

        #endregion Private Methods
    }
}