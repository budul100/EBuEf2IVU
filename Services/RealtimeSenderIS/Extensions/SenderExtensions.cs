using System;
using System.Collections.Generic;
using System.Linq;
using EBuEf2IVU.Shareds.Commons.Enums;
using EBuEf2IVU.Shareds.Commons.Models;
using RealtimeSenderIS;

namespace EBuEf2IVU.Services.RealtimeSenderIS.Extensions
{
    internal static class SenderExtensions
    {
        #region Private Fields

        private static readonly DateTime timestampSubtract = GetTimestampSubtract();

        #endregion Private Fields

        #region Public Methods

        public static int GetClassifier(this TrainLeg leg)
        {
            var result = leg.IstPrognose
                ? RealtimeInfoConstants.ClassifierPrognosis
                : RealtimeInfoConstants.ClassifierActual;

            return result;
        }

        public static int GetEventcode(this TrainLeg leg)
        {
            return leg.IVULegTyp switch
            {
                LegType.Durchfahrt => RealtimeInfoConstants.EventCodePassing,
                LegType.Ankunft => RealtimeInfoConstants.EventCodeArrival,
                _ => RealtimeInfoConstants.EventCodeDeparture,
            };
        }

        public static int GetTrainCombinationComplete(this IEnumerable<VehicleTO> vehicles)
        {
            return vehicles.Any()
                ? RealtimeInfoConstants.TrainCombinationUnknown
                : RealtimeInfoConstants.TrainCombinationComplete;
        }

        public static IEnumerable<VehicleTO> GetVehicleTOs(this IEnumerable<string> vehicles)
        {
            if (vehicles?.Any() ?? false)
            {
                var position = 0;

                foreach (var vehicle in vehicles)
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