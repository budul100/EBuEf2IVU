using Common.BusinessObjects;
using Common.Interfaces;
using Polly;
using Polly.Retry;
using RealtimeSender.Extensions;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace RealtimeSender
{
    public class SenderManager : ISenderManager
    {
        #region Private Fields

        private const string EnvironmentComputer = "COMPUTERNAME";

        private const int RT2IVUEventCodeAllocation = 9;
        private const int RT2IVUShuntingTrip = 0;
        private const int RT2IVUTrackPosition = 0;
        private readonly CancellationToken cancellationToken;
        private readonly string deviceID;
        private readonly ConcurrentQueue<RealTimeInfoTO> infosQueue = new ConcurrentQueue<RealTimeInfoTO>();
        private readonly ILogger logger;

        private string division;
        private string endpoint;
        private AsyncRetryPolicy retryPolicy;

        #endregion Private Fields

        #region Public Constructors

        public SenderManager(ILogger logger, CancellationToken cancellationToken)
        {
            this.logger = logger;
            this.cancellationToken = cancellationToken;

            deviceID = Environment.GetEnvironmentVariable(EnvironmentComputer);
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

                if (info != null)
                    infosQueue.Enqueue(info);
            }
        }

        public void AddRealtime(TrainPosition position)
        {
            if (position != null)
            {
                var info = GetRealtimeInfo(
                    eventCode: position.GetEventcode(),
                    tripNumber: position.Zugnummer,
                    timeStamp: position.IVUZeitpunkt,
                    stopArea: position.IVUNetzpunkt,
                    track: position.IVUGleis,
                    vehicles: position.Fahrzeuge);

                if (info != null)
                    infosQueue.Enqueue(info);
            }
        }

        public void Run(int retryTime, string endpoint, string division)
        {
            this.endpoint = endpoint;
            this.division = division;

            retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    sleepDurationProvider: (p) => TimeSpan.FromSeconds(retryTime),
                    onRetry: (exception, reconnection) => OnRetry(exception, reconnection));

            retryPolicy.ExecuteAsync(
                action: (t) => Task.Run(() => RunSender(t)),
                cancellationToken: cancellationToken);
        }

        #endregion Public Methods

        #region Private Methods

        private static IEnumerable<VehicleTO> GetVehicleTOs
            (IEnumerable<string> vehicles)
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

        private RealTimeInfoTO GetRealtimeInfo(int eventCode, string tripNumber, DateTime timeStamp, string stopArea,
            string track, IEnumerable<string> vehicles)
        {
            var result = default(RealTimeInfoTO);

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
                logger.Error(
                    $"Fehler beim Erzeugen einer Ist-Zeit-Nachrichten für IVU.rail: {ex.Message}");
            }
            return result;
        }

        private void OnRetry(Exception exception, TimeSpan reconnection)
        {
            logger.Error($"Fehler beim Senden der Ist-Zeiten an IVU.rail: {exception.Message}\r\n" +
                $"Die Verbindung wird in {reconnection.TotalSeconds} Sekunden wieder versucht.");
        }

        private void RunSender(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!infosQueue.IsEmpty)
                {
                    var current = GetFirstInQueue().ToArray();

                    using (var client = new RealTimeInformationImportFacadeClient())
                    {
                        client.Endpoint.Address = new EndpointAddress(endpoint ?? string.Empty);

                        var response = Task.Run(() => client.importRealTimeInfoAsync(current));
                        var validation = response.Result.importRealTimeInfoResponse1;

                        if (validation.GetLength(0) > 0)
                        {
                            if (validation[0].code == 0)
                            {
                                logger.Debug("Ist-Zeit-Nachricht wurde erfolgreich an IVU.rail gesendet.");
                            }
                            else
                            {
                                logger.Error($"Fehlermeldung zur Ist-Zeit-Nachricht von IVU.rail empfangen: {validation[0].message}");
                            }
                        }

                        infosQueue.TryDequeue(out RealTimeInfoTO info);
                    }
                }
            }
        }

        #endregion Private Methods
    }
}