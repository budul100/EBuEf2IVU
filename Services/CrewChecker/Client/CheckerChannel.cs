using CredentialChannelFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrewChecker.Client
{
    internal class CheckerChannel
    {
        #region Private Fields

        private readonly Factory<CrewOnTripPortTypeChannel> channelFactory;
        private readonly string division;
        private readonly string planningLevel;

        #endregion Private Fields

        #region Public Constructors

        public CheckerChannel(string host, int port, string path, string userName, string password, bool isHttps,
            bool ignoreCertificateErrors, string division, string planningLevel)
        {
            this.division = division;
            this.planningLevel = planningLevel;

            channelFactory = new Factory<CrewOnTripPortTypeChannel>(
                host: host,
                port: port,
                path: path,
                userName: userName,
                password: password,
                isHttps: isHttps,
                notIgnoreCertificateErrors: ignoreCertificateErrors);
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task<IEnumerable<tripAssignment>> GetAssignmentsAsync(IEnumerable<string> tripNumbers, DateTime date)
        {
            var result = default(IEnumerable<tripAssignment>);

            var request = GetRequest(
                tripNumbers: tripNumbers,
                date: date);

            using (var channel = channelFactory.Get())
            {
                var response = await channel.exportCrewAssignmentsForTripsAsync(request);

                if (response.exportCrewAssignmentsResponse.error != default)
                {
                    throw new ApplicationException($"Error response received at crew on trip " +
                        $"request:{response.exportCrewAssignmentsResponse.error.description}");
                }

                result = response.exportCrewAssignmentsResponse.tripAssignment.ToArray();
            }

            return result;
        }

        #endregion Public Methods

        #region Private Methods

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

        #endregion Private Methods
    }
}