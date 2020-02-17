using Common.Models;
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

        public async Task<IEnumerable<CrewingElement>> GetAsync(IEnumerable<string> tripNumbers, DateTime date)
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

            var result = GetCrewingElements(response.exportCrewAssignmentsResponse.tripAssignment).ToArray();

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private IEnumerable<CrewingElement> GetCrewingElements(IEnumerable<tripAssignment> tripAssignments)
        {
            if (tripAssignments?.Any() ?? false)
            {
                foreach (var tripAssignment in tripAssignments)
                {
                    var result = new CrewingElement
                    {
                        BetriebsstelleVon = tripAssignment.employeeOrigin,
                        BetriebsstelleNach = tripAssignment.employeeDestination,
                        DienstKurzname = tripAssignment.duty,
                        PersonalNachname = tripAssignment.name,
                        PersonalNummer = tripAssignment.personnelNumber,
                        Zugnummer = tripAssignment.trip,
                        ZugnummerVorgaenger = tripAssignment.previousTripNumber,
                    };

                    yield return result;
                }
            }
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

        #endregion Private Methods
    }
}