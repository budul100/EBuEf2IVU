#pragma warning disable CA1031 // Do not catch general exception types

using Common.Models;
using Microsoft.Extensions.Logging;
using RealtimeSender.Extensions;
using RealtimeSender20;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealtimeSender.Converters
{
    internal class Message2RealtimeInfo
    {
        #region Private Fields

        private const string EnvironmentComputer = "COMPUTERNAME";

        private readonly string deviceID;
        private readonly string division;
        private readonly ILogger logger;
        private readonly DateTime sessionStart;

        #endregion Private Fields

        #region Public Constructors

        public Message2RealtimeInfo(ILogger logger, string division, DateTime sessionStart)
        {
            this.logger = logger;
            this.division = division;
            this.sessionStart = sessionStart;

            deviceID = Environment.GetEnvironmentVariable(EnvironmentComputer);
        }

        #endregion Public Constructors

        #region Public Methods

        public RealTimeInfoTO Get(VehicleAllocation allocation)
        {
            var result = GetRealtimeInfo(
                eventCode: RealtimeInfoConstants.EventCodeAllocation,
                classifier: RealtimeInfoConstants.ClassifierActual,
                tripNumber: allocation.Zugnummer,
                timeStamp: sessionStart,
                stopArea: allocation.Betriebsstelle,
                track: allocation.Gleis,
                vehicles: allocation.Fahrzeuge);

            return result;
        }

        public RealTimeInfoTO Get(TrainLeg leg)
        {
            var result = GetRealtimeInfo(
                eventCode: leg.GetEventcode(),
                classifier: leg.GetClassifier(),
                tripNumber: leg.Zugnummer,
                timeStamp: leg.IVUZeitpunkt,
                stopArea: leg.IVUNetzpunkt,
                track: leg.IVUGleis,
                vehicles: leg.Fahrzeuge);

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private RealTimeInfoTO GetRealtimeInfo(int eventCode, int classifier, string tripNumber, DateTime timeStamp,
            string stopArea, string track, IEnumerable<string> vehicles)
        {
            var result = default(RealTimeInfoTO);

            if (!string.IsNullOrWhiteSpace(stopArea))
            {
                try
                {
                    var unixTimeStamp = timeStamp.ToUnixTimestamp();

                    result = new RealTimeInfoTO
                    {
                        classifier = classifier,
                        classifierSpecified = true,
                        deviceId = deviceID,
                        division = division,
                        //employeeId = this.config.User,
                        eventCode = eventCode,
                        stopArea = stopArea,
                        timeStamp = unixTimeStamp,
                        trainCombinationCompleteSpecified = true,
                        tripIdentificationDate = unixTimeStamp,
                        tripIdentificationDateSpecified = true,
                        tripNumber = tripNumber,
                    };

                    if (!string.IsNullOrEmpty(track))
                    {
                        result.track = track;
                        result.trackposition = RealtimeInfoConstants.TrackPosition;
                        result.trackpositionSpecified = true;
                        result.shuntingTrip = RealtimeInfoConstants.ShuntingTrip;
                        result.shuntingTripSpecified = true;
                    }

                    result.vehicles = vehicles.GetVehicleTOs().ToArray();
                    result.trainCombinationComplete = result.vehicles.GetTrainCombinationComplete();

                    return result;
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        "Fehler beim Erzeugen einer Ist-Zeit-Nachrichten für IVU.rail: {message}",
                        ex.Message);
                }
            }

            return result;
        }

        #endregion Private Methods
    }
}

#pragma warning restore CA1031 // Do not catch general exception types