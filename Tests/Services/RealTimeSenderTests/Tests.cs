using Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using RealtimeSender;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeSenderTests
{
    public class Tests
    {
        #region Private Fields

        private IConfigurationRoot config;
        private NullLoggerFactory loggerFactory;
        private Sender sender;

        #endregion Private Fields

        #region Public Methods

        [SetUp]
        public void Init()
        {
            var path = Path.GetFullPath(@"..\..\..\..\..\..\Programs\EBuEf2IVUVehicle\ebuef2ivuvehicle-settings.example.xml");

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddXmlFile(
                path: path,
                optional: false,
                reloadOnChange: false);

            config = configBuilder.Build();

            loggerFactory = new NullLoggerFactory();
            var logger = loggerFactory.CreateLogger<Sender>();

            var senderSettings = config
                .GetSection(nameof(EBuEf2IVUVehicle.Settings.RealtimeSender))
                .Get<EBuEf2IVUVehicle.Settings.RealtimeSender>();

            sender = new Sender(logger);

            sender.Initialize(
                host: senderSettings.Host,
                port: senderSettings.Port,
                path: senderSettings.Path,
                username: senderSettings.Username,
                password: senderSettings.Password,
                isHttps: senderSettings.IsHttps,
                division: senderSettings.Division,
                sessionStart: DateTime.Now,
                retryTime: senderSettings.RetryTime);
        }

        [Test]
        public void SendRealtime()
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var trainLeg = new TrainLeg
            {
                IVUNetzpunkt = "ABC",
                IVUZeitpunkt = DateTime.Now,
                IVUGleis = "123",
                IVULegTyp = Common.Enums.LegType.Abfahrt,
            };

            sender.Add(trainLeg);

            Task.WhenAny(
                _ = sender.ExecuteAsync(token));

            Thread.Sleep(6000);

            tokenSource.Cancel();
        }

        #endregion Public Methods
    }
}