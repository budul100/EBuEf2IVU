using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using RealtimeSenderIS;
using EBuEf2IVU.Services.RealtimeSenderIS.Extensions;
using EBuEf2IVU.Shareds.Commons.Models;

namespace EBuEf2IVU.Services.RealtimeSenderIS.Converters
{
    internal class Message2RealtimeInfo(ILogger logger, string division)
    {
        #region Private Fields

        private const string EnvironmentComputer = "COMPUTERNAME";

        private readonly string deviceID = Environment.GetEnvironmentVariable(EnvironmentComputer);
        private DateTime ivuDatum;
        private TimeSpan sessionStart;

        #endregion Private Fields

        #region Public Methods

        public RealTimeInfoTO Convert(VehicleAllocation allocation)
        {
            var ivuTimestamp = ivuDatum.Add(sessionStart);

            var result = GetRealtimeInfo(
                eventCode: RealtimeInfoConstants.EventCodeAllocation,
                classifier: RealtimeInfoConstants.ClassifierActual,
                tripNumber: allocation.Zugnummer,
                timestamp: ivuTimestamp,
                stopArea: allocation.Betriebsstelle,
                track: allocation.Gleis,
                vehicles: allocation.Fahrzeuge);

            return result;
        }

        public RealTimeInfoTO Convert(TrainLeg leg)
        {
            var ivuTimestamp = ivuDatum.Add(leg.IVUZeit);

            var result = GetRealtimeInfo(
                eventCode: leg.GetEventcode(),
                classifier: leg.GetClassifier(),
                tripNumber: leg.Zugnummer,
                timestamp: ivuTimestamp,
                stopArea: leg.IVUNetzpunkt,
                track: leg.IVUGleis,
                vehicles: leg.Fahrzeuge);

            return result;
        }

        public void Initialize(DateTime ivuDatum, TimeSpan sessionStart)
        {
            this.ivuDatum = ivuDatum;
            this.sessionStart = sessionStart;
        }

        #endregion Public Methods

        #region Private Methods

        private RealTimeInfoTO GetRealtimeInfo(int eventCode, int classifier, string tripNumber, DateTime timestamp,
            string stopArea, string track, IEnumerable<string> vehicles)
        {
            var result = default(RealTimeInfoTO);

            if (!string.IsNullOrWhiteSpace(stopArea))
            {
                try
                {
                    var unixTimeStamp = timestamp.ToUnixTimestamp();

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