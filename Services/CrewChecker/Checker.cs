using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Commons.Interfaces;
using Commons.Models;
using CredentialChannelFactory;
using CrewChecker.Extensions;
using EnumerableExtensions;
using Polly;
using Polly.Retry;

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
                action: _ => GetCrewsForTrips(
                    tripNumbers: tripNumbers,
                    date: date),
                cancellationToken: cancellationToken);

            return result;
        }

        public void Initialize(string host, int port, bool isHttps, string username, string password, string path,
            string division, string planningLevel, int retryTime)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentException(
                    $"\"{nameof(host)}\" darf nicht NULL oder ein Leerraumzeichen sein.",
                    nameof(host));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(
                    $"\"{nameof(path)}\" kann nicht NULL oder leer sein.",
                    nameof(path));
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException(
                    $"\"{nameof(username)}\" kann nicht NULL oder leer sein.",
                    nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(
                    $"\"{nameof(password)}\" darf nicht NULL oder ein Leerraumzeichen sein.",
                    nameof(password));
            }

            if (string.IsNullOrWhiteSpace(division))
            {
                throw new ArgumentException(
                    $"\"{nameof(division)}\" darf nicht NULL oder ein Leerraumzeichen sein.",
                    nameof(division));
            }

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

            logger.LogDebug(
                "Die Crew-on-trip-Anfragen werden gesendet an: {uri}",
                channelFactory.Uri.AbsoluteUri);

            retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    sleepDurationProvider: _ => TimeSpan.FromSeconds(retryTime),
                    onRetry: OnRetry);
        }

        #endregion Public Methods

        #region Private Methods

        private async Task<IEnumerable<CrewingElement>> GetCrewsForTrips(IEnumerable<string> tripNumbers, DateTime date)
        {
            var result = default(IEnumerable<CrewingElement>);

            var relevants = tripNumbers.IfAny()
                .Where(t => !string.IsNullOrWhiteSpace(t)).ToArray();

            if (relevants.AnyItem())
            {
                var request = GetRequest(
                    tripNumbers: relevants,
                    date: date);

                using var channel = channelFactory.Get();

                var response = await channel.exportCrewAssignmentsForTripsAsync(request);

                if (response?.exportCrewAssignmentsResponse != default)
                {
                    if (response.exportCrewAssignmentsResponse.error != default)
                    {
                        throw new ApplicationException(
                            $"Die Crew-on-trip-Anfrage hat eine Fehlermeldung erhalten: {response.exportCrewAssignmentsResponse.error.description}");
                    }

                    var assignments = response.exportCrewAssignmentsResponse.tripAssignment?.ToArray();

                    result = assignments.GetCrewingElements().ToArray();
                }
            }

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
            while (exception.InnerException != default) exception = exception.InnerException;

            logger.LogError(
                exception,
                "Fehler beim Abrufen der Crew-Informationen von IVU.rail: {exceptionMessage}\r\n" +
                "Die Verbindung wird in {reconnectionPeriod} Sekunden wieder versucht.",
                exception.Message,
                reconnection.TotalSeconds);

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        #endregion Private Methods
    }
}