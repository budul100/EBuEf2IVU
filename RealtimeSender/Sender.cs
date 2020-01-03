using Common.Interfaces;
using Common.Models;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RealtimeSender.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace RealtimeSender
{
    public class Sender
        : ISender
    {
        #region Private Fields

        private const string EnvironmentComputer = "COMPUTERNAME";

        private const int RT2IVUEventCodeAllocation = 9;
        private const int RT2IVUShuntingTrip = 0;
        private const int RT2IVUTrackPosition = 0;

        private readonly string deviceID;
        private readonly string division;
        private readonly EndpointAddress endpointAddress;
        private readonly ConcurrentQueue<RealTimeInfoTO> infosQueue = new ConcurrentQueue<RealTimeInfoTO>();
        private readonly ILogger logger;
        private readonly AsyncRetryPolicy retryPolicy;

        #endregion Private Fields

        #region Public Constructors

        public Sender(ILogger logger, string division, string endpoint, int retryTime)
        {
            this.logger = logger;
            this.division = division;

            endpointAddress = new EndpointAddress(endpoint ?? string.Empty);
            deviceID = Environment.GetEnvironmentVariable(EnvironmentComputer);

            retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    sleepDurationProvider: (p) => TimeSpan.FromSeconds(retryTime),
                    onRetry: (exception, reconnection) => OnRetry(
                        exception: exception,
                        reconnection: reconnection));
        }

        #endregion Public Constructors

        #region Public Methods

        public void AddAllocations(IEnumerable<VehicleAllocation> allocations)
        {
            foreach (var allocation in allocations)
            {
                var info = GetRealtimeInfo(
                    eventCode: RT2IVUEventCodeAllocation,
                    tripNumber: allocation.Zugnummer,
                    timeStamp: DateTime.Now,
                    stopArea: allocation.Betriebsstelle,
                    track: allocation.Gleis,
                    vehicles: allocation.Fahrzeuge);

                if (info != default)
                    infosQueue.Enqueue(info);
            }
        }

        public void AddRealtime(TrainPosition position)
        {
            if (position != default)
            {
                var info = GetRealtimeInfo(
                    eventCode: position.GetEventcode(),
                    tripNumber: position.Zugnummer,
                    timeStamp: position.IVUZeitpunkt,
                    stopArea: position.IVUNetzpunkt,
                    track: position.IVUGleis,
                    vehicles: position.Fahrzeuge);

                if (info != default)
                    infosQueue.Enqueue(info);
            }
        }

        public Task RunAsnc(CancellationToken cancellationToken)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (t) => RunSenderAsync(t),
                cancellationToken: cancellationToken);

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private static IEnumerable<VehicleTO> GetVehicleTOs(IEnumerable<string> vehicles)
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

        private IEnumerable<RealTimeInfoTO> GetFirstInQueue()
        {
            if (!infosQueue.IsEmpty)
            {
                infosQueue.TryPeek(out RealTimeInfoTO info);

                if (info != null)
                    yield return info;
            }
        }

        private RealTimeInfoTO GetRealtimeInfo(int eventCode, string tripNumber, DateTime timeStamp,
            string stopArea, string track, IEnumerable<string> vehicles)
        {
            var result = default(RealTimeInfoTO);

            if (!string.IsNullOrWhiteSpace(stopArea))
            {
                try
                {
                    result = new RealTimeInfoTO
                    {
                        deviceId = deviceID,
                        division = division,
                        //employeeId = this.config.User,
                        eventCode = eventCode,
                        stopArea = stopArea,
                        timeStamp = timeStamp.ToUnixTimestamp(),
                        trainCombinationCompleteSpecified = true,
                        tripIdentificationDate = timeStamp.ToUnixTimestamp(),
                        tripIdentificationDateSpecified = true,
                        tripNumber = tripNumber,
                    };

                    if (!string.IsNullOrEmpty(track))
                    {
                        result.track = track;
                        result.trackposition = RT2IVUTrackPosition;
                        result.trackpositionSpecified = true;
                        result.shuntingTrip = RT2IVUShuntingTrip;
                        result.shuntingTripSpecified = true;
                    }

                    result.vehicles = GetVehicleTOs(vehicles).ToArray();
                    result.trainCombinationComplete = result.vehicles.GetTrainCombinationComplete();

                    return result;
                }
                catch (Exception ex)
                {
                    logger.LogError($"Fehler beim Erzeugen einer Ist-Zeit-Nachrichten für IVU.rail: {ex.Message}");
                }
            }

            return result;
        }

        private void OnRetry(Exception exception, TimeSpan reconnection)
        {
            logger.LogError(
                $"Fehler beim Senden der Ist-Zeiten an IVU.rail: {exception.Message}\r\n" +
                $"Die Verbindung wird in {reconnection.TotalSeconds} Sekunden wieder versucht.");
        }

        private async Task RunSenderAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!infosQueue.IsEmpty)
                {
                    var current = GetFirstInQueue().ToArray();

                    using (var client = new RealTimeInformationImportFacadeClient())
                    {
                        client.Endpoint.Address = endpointAddress;

                        var response = await client.importRealTimeInfoAsync(current);
                        var result = response.importRealTimeInfoResponse1;

                        if (result.Any() && result.First().code == 0)
                        {
                            logger.LogDebug("Ist-Zeit-Nachricht wurde erfolgreich an IVU.rail gesendet.");
                        }
                        else if (result.Any())
                        {
                            logger.LogError($"Fehlermeldung zur Ist-Zeit-Nachricht von IVU.rail empfangen: {result[0].message}");
                        }

                        infosQueue.TryDequeue(out RealTimeInfoTO info);
                    }
                }
            }
        }

        #endregion Private Methods
    }
}