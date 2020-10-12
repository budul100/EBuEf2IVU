using Common.Models;
using Microsoft.Extensions.Logging;
using RealtimeSender.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealtimeSender.Converters
{
    internal class Messages2RealtimeInfo
    {
        #region Private Fields

        private const string EnvironmentComputer = "COMPUTERNAME";

        private const int EventCodeAllocation = 9;
        private const int ShuntingTrip = 0;
        private const int TrackPosition = 0;

        private readonly string deviceID;
        private readonly string division;
        private readonly ILogger logger;
        private readonly DateTime sessionStart;

        #endregion Private Fields

        #region Public Constructors

        public Messages2RealtimeInfo(ILogger logger, string division, DateTime sessionStart)
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
                eventCode: EventCodeAllocation,
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
                tripNumber: leg.Zugnummer,
                timeStamp: leg.IVUZeitpunkt,
                stopArea: leg.IVUNetzpunkt,
                track: leg.IVUGleis,
                vehicles: leg.Fahrzeuge);

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private RealTimeInfoTO GetRealtimeInfo(int eventCode, string tripNumber, DateTime timeStamp,
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
                        result.trackposition = TrackPosition;
                        result.trackpositionSpecified = true;
                        result.shuntingTrip = ShuntingTrip;
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