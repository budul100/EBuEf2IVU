using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrewChecker.Client
{
    internal class CheckerChannel
        : BaseChannel<CrewOnTripPortTypeChannel>
    {
        #region Private Fields

        private readonly string division;
        private readonly string planningLevel;

        #endregion Private Fields

        #region Public Constructors

        public CheckerChannel(string host, int port, string path, string userName, string password, bool isHttps,
            bool ignoreCertificateErrors, string division, string planningLevel)
            : base(host, port, path, userName, password, isHttps, ignoreCertificateErrors)
        {
            this.division = division;
            this.planningLevel = planningLevel;
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task<IEnumerable<tripAssignment>> GetAssignmentsAsync(IEnumerable<string> tripNumbers, DateTime date)
        {
            var request = GetRequest(
                tripNumbers: tripNumbers,
                date: date);

            var response = await Channel.exportCrewAssignmentsForTripsAsync(request);

            if (response.exportCrewAssignmentsResponse.error != default)
            {
                throw new ApplicationException($"Error response received at crew on trip " +
                    $"request:{response.exportCrewAssignmentsResponse.error.description}");
            }

            var result = response.exportCrewAssignmentsResponse.tripAssignment.ToArray();

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