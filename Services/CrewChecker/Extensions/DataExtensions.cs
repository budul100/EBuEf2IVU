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
            if (tripAssignments.AnyItem())
            {
                var employeeAssignments = tripAssignments
                    .Where(a => a.employeeOrigin != default
                        && a.employeeDestination != default).ToArray();

                foreach (var employeeAssignment in employeeAssignments)
                {
                    var result = new CrewingElement
                    {
                        BetriebsstelleVon = employeeAssignment.employeeOrigin,
                        BetriebsstelleNach = employeeAssignment.employeeDestination,
                        DienstKurzname = employeeAssignment.duty,
                        PersonalNachname = employeeAssignment.surname,
                        PersonalNummer = employeeAssignment.personnelNumber,
                        Zugnummer = employeeAssignment.trip,
                        ZugnummerVorgaenger = employeeAssignment.previousTripNumber,
                    };

                    yield return result;
                }
            }
        }

        #endregion Public Methods
    }
}