using Common.Models;
using Microsoft.Extensions.Logging;
using RealtimeSender.Extensions;
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

        #endregion Private Fields

        #region Public Constructors

        public Message2RealtimeInfo(ILogger logger, string division)
        {
            this.logger = logger;
            this.division = division;

            deviceID = Environment.GetEnvironmentVariable(EnvironmentComputer);
        }

        #endregion Public Constructors

        #region Public Methods

        public RealTimeInfoTO Get(VehicleAllocation allocation, DateTime sessionStart)
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
                catch (Exception exception)
                {
                    logger.LogError(
                        exception,
                        "Fehler beim Erzeugen einer Ist-Zeit-Nachrichten für IVU.rail: {message}",
                        exception.Message);
                }
            }

            return result;
        }

        #endregion Private Methods
    }
}