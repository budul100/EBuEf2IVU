using Common.Enums;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealtimeSender.Extensions
{
    internal static class SenderExtensions
    {
        #region Private Fields

        private const int EventCodeArrival = 1;
        private const int EventCodeDeparture = 2;
        private const int EventCodePassing = 3;
        private const int TrainCombinationComplete = 1;
        private const int TrainCombinationUnknown = 2;

        private static readonly DateTime timestampSubtract = GetTimestampSubtract();

        #endregion Private Fields

        #region Public Methods

        public static int GetEventcode(this TrainLeg leg)
        {
            return leg.IVULegTyp switch
            {
                LegType.Abfahrt => EventCodeDeparture,
                LegType.Ankunft => EventCodeArrival,
                _ => EventCodePassing,
            };
        }

        public static int GetTrainCombinationComplete(this IEnumerable<VehicleTO> vehicles)
        {
            return vehicles.Any()
                ? TrainCombinationUnknown
                : TrainCombinationComplete;
        }

        public static IEnumerable<VehicleTO> GetVehicleTOs(this IEnumerable<string> vehicles)
        {
            var position = 0;
            foreach (string vehicle in vehicles)
            {
                if (!string.IsNullOrEmpty(vehicle))
                {
                    var result = new VehicleTO
                    {
                        orientation = 0,
                        position = ++position,
                        positionSpecified = true,
                        number = vehicle
                    };

                    yield return result;
                }
            }
        }

        public static long ToUnixTimestamp(this DateTime originalDate)
        {
            return (long)originalDate.Subtract(timestampSubtract).TotalMilliseconds;
        }

        #endregion Public Methods

        #region Private Methods

        private static DateTime GetTimestampSubtract()
        {
            var baseDate = new DateTime(
                year: 1970,
                month: 1,
                day: 1,
                hour: 0,
                minute: 0,
                second: 0,
                kind: DateTimeKind.Unspecified);

            var result = baseDate.Add(DateTime.Now - DateTime.UtcNow);

            return result;
        }

        #endregion Private Methods
    }
}