using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealtimeSender.Extensions
{
    internal static class RealtimeInfoExtensions
    {
        #region Private Fields

        private const int RT2IVUEventCodeArrival = 1;
        private const int RT2IVUEventCodeDeparture = 2;
        private const int RT2IVUEventCodePassing = 3;
        private const int RT2IVUTrainCombinationComplete = 1;
        private const int RT2IVUTrainCombinationUnknown = 2;

        private static readonly DateTime timestampSubtract;

        #endregion Private Fields

        #region Public Constructors

        static RealtimeInfoExtensions()
        {
            timestampSubtract = GetTimestampSubtract();
        }

        #endregion Public Constructors

        #region Public Methods

        public static int GetEventcode(this TrainPosition trainPosition)
        {
            switch (trainPosition.IVUTrainPositionTyp)
            {
                case TrainPositionType.Abfahrt:
                    return RT2IVUEventCodeDeparture;

                case TrainPositionType.Ankunft:
                    return RT2IVUEventCodeArrival;

                default:
                    return RT2IVUEventCodePassing;
            }
        }

        public static int GetTrainCombinationComplete(this IEnumerable<VehicleTO> vehicles)
        {
            return vehicles.Any()
                ? RT2IVUTrainCombinationUnknown
                : RT2IVUTrainCombinationComplete;
        }

        public static long ToUnixTimestamp(this DateTime originalDate)
        {
            return (long)originalDate.Subtract(timestampSubtract).TotalMilliseconds;
        }

        #endregion Public Methods

        #region Private Methods

        private static DateTime GetTimestampSubtract()
        {
            return new DateTime(
                year: 1970,
                month: 1,
                day: 1,
                hour: 0,
                minute: 0,
                second: 0,
                kind: DateTimeKind.Unspecified)
                .Add(DateTime.Now - DateTime.UtcNow);
        }

        #endregion Private Methods
    }
}