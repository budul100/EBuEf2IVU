using Common.Models;
using EnumerableExtensions;
using System.Collections.Generic;
using System.Linq;

namespace CrewChecker.Extensions
{
    internal static class DataExtensions
    {
        #region Public Methods

        public static IEnumerable<CrewingElement> GetCrewingElements(this IEnumerable<tripAssignment> tripAssignments)
        {
            var relevants = tripAssignments.IfAny()
                .Where(a => a?.employeeOrigin != default
                    && a?.employeeDestination != default).ToArray();

            foreach (var relevant in relevants)
            {
                var result = new CrewingElement
                {
                    BetriebsstelleVon = relevant.employeeOrigin,
                    BetriebsstelleNach = relevant.employeeDestination,
                    DienstKurzname = relevant.duty,
                    PersonalNachname = relevant.surname,
                    PersonalNummer = relevant.personnelNumber,
                    Zugnummer = relevant.trip,
                    ZugnummerVorgaenger = relevant.previousTripNumber,
                };

                yield return result;
            }
        }

        #endregion Public Methods
    }
}