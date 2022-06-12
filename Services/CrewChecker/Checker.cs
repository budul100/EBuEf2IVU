using Common.Interfaces;
using Common.Models;
using CredentialChannelFactory;
using CrewChecker.Extensions;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CrewChecker
{
    public class Checker
        : ICrewChecker
    {
        #region Private Fields

        private readonly ILogger logger;

        private Factory<CrewOnTripPortTypeChannel> channelFactory;
        private string division;
        private string planningLevel;
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
                action: _ => GetAsync(
                    tripNumbers: tripNumbers,
                    date: date),
                cancellationToken: cancellationToken);

            return result;
        }

        public void Initialize(string host, int port, string path, string username, string password, bool isHttps,
            int retryTime, string division, string planningLevel)
        {
            this.division = division;
            this.planningLevel = planningLevel;

            channelFactory = new Factory<CrewOnTripPortTypeChannel>(
                host: host,
                port: port,
                path: path,
                userName: username,
                password: password,
                isHttps: isHttps,
                notIgnoreCertificateErrors: true);

            retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    sleepDurationProvider: _ => TimeSpan.FromSeconds(retryTime),
                    onRetry: (exception, reconnection) => OnRetry(
                        exception: exception,
                        reconnection: reconnection));
        }

        #endregion Public Methods

        #region Private Methods

        private async Task<IEnumerable<CrewingElement>> GetAsync(IEnumerable<string> tripNumbers, DateTime date)
        {
            var assignments = default(IEnumerable<tripAssignment>);

            var request = GetRequest(
                tripNumbers: tripNumbers,
                date: date);

            using (var channel = channelFactory.Get())
            {
                var response = await channel.exportCrewAssignmentsForTripsAsync(request);

                if (response.exportCrewAssignmentsResponse.error != default)
                {
                    throw new ApplicationException(
                        $"Error response received at crew on trip request:{response.exportCrewAssignmentsResponse.error.description}");
                }

                assignments = response.exportCrewAssignmentsResponse.tripAssignment.ToArray();
            }

            var result = assignments.GetCrewingElements().ToArray();

            return result;
        }

        private exportCrewAssignmentsForTrips GetRequest(IEnumerable<string> tripNumbers, DateTime date)
        {
            var request = new exportCrewAssignmentsForTripsRequest
            {
                division = division,
                from = date,
                planningLevel = planningLevel,
                to = date,
                tripNumbers = tripNumbers.ToArray(),
                withPassengers = false,
            };

            var result = new exportCrewAssignmentsForTrips(request);

            return result;
        }

        private void OnRetry(Exception exception, TimeSpan reconnection)
        {
            while (exception.InnerException != null) exception = exception.InnerException;

            logger.LogError(
                "Fehler beim Abrufen der Crew-Informationen von IVU.rail: {exceptionMessage}\r\n" +
                "Die Verbindung wird in {reconnectionPeriod} Sekunden wieder versucht.",
                exception.Message,
                reconnection.TotalSeconds);
        }

        #endregion Private Methods
    }
}