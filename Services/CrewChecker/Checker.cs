using Common.Interfaces;
using Common.Models;
using CrewChecker.Client;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrewChecker
{
    public class Checker
        : ICrewChecker
    {
        #region Private Fields

        private readonly ILogger logger;

        private CheckerChannel client;
        private AsyncRetryPolicy retryPolicy;

        #endregion Private Fields

        #region Public Constructors

        public Checker(ILogger<Checker> logger)
        {
            this.logger = logger;
        }

        #endregion Public Constructors

        #region Public Methods

        public Task<IEnumerable<CrewingElement>> GetCrewingElementsAsync(IEnumerable<string> tripNumbers, DateTime date,
            CancellationToken cancellationToken)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (token) => client.GetAsync(
                    tripNumbers: tripNumbers,
                    date: date),
                cancellationToken: cancellationToken);

            return result;
        }

        public void Initialize(string host, int port, string path, string username, string password, bool isHttps,
            string division, string planningLevel, int retryTime)
        {
            client = new CheckerChannel(
                host: host,
                port: port,
                path: path,
                userName: username,
                password: password,
                isHttps: isHttps,
                ignoreCertificateErrors: true,
                division: division,
                planningLevel: planningLevel);

            retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    sleepDurationProvider: (p) => TimeSpan.FromSeconds(retryTime),
                    onRetry: (exception, reconnection) => OnRetry(
                        exception: exception,
                        reconnection: reconnection));
        }

        #endregion Public Methods

        #region Private Methods

        private void OnRetry(Exception exception, TimeSpan reconnection)
        {
            logger.LogError(
                $"Fehler beim Abrufen der Crew-Informationen von IVU.rail: {exception.Message}\r\n" +
                $"Die Verbindung wird in {reconnection.TotalSeconds} Sekunden wieder versucht.");
        }

        #endregion Private Methods
    }
}