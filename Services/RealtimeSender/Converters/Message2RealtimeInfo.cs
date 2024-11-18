using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Commons.Models;
using RealtimeSender.Extensions;

namespace RealtimeSender.Converters
{
    internal class Message2RealtimeInfo(ILogger logger, string division)
    {
        #region Private Fields

        private const string EnvironmentComputer = "COMPUTERNAME";

        private readonly string deviceID = Environment.GetEnvironmentVariable(EnvironmentComputer);

        private DateTime? ivuDatum;
        private TimeSpan sessionStart;

        #endregion Private Fields

        #region Public Methods

        public RealTimeInfoTO Convert(VehicleAllocation allocation)
        {
            var result = default(RealTimeInfoTO);

            if (ivuDatum.HasValue)
            {
                var ivuTimestamp = ivuDatum.Value.Add(sessionStart);

                result = GetRealtimeInfo(
                    eventCode: RealtimeInfoConstants.EventCodeAllocation,
                    classifier: RealtimeInfoConstants.ClassifierActual,
                    tripNumber: allocation.Zugnummer,
                    timeStamp: ivuTimestamp,
                    stopArea: allocation.Betriebsstelle,
                    track: allocation.Gleis,
                    vehicles: allocation.Fahrzeuge);
            }
            else
            {
                logger.LogWarning(
                    message: "Der Konverter für Realtime-Messages wurde noch nicht initialisiert.");
            }

            return result;
        }

        public RealTimeInfoTO Convert(TrainLeg leg)
        {
            var result = default(RealTimeInfoTO);

            if (ivuDatum.HasValue)
            {
                var ivuTimestamp = ivuDatum.Value.Add(leg.IVUZeit);

                result = GetRealtimeInfo(
                    eventCode: leg.GetEventcode(),
                    classifier: leg.GetClassifier(),
                    tripNumber: leg.Zugnummer,
                    timeStamp: ivuTimestamp,
                    stopArea: leg.IVUNetzpunkt,
                    track: leg.IVUGleis,
                    vehicles: leg.Fahrzeuge);
            }
            else
            {
                logger.LogWarning(
                    message: "Der Konverter für Realtime-Messages wurde noch nicht initialisiert.");
            }

            return result;
        }

        public void Initialize(DateTime ivuDatum, TimeSpan sessionStart)
        {
            this.ivuDatum = ivuDatum;
            this.sessionStart = sessionStart;
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