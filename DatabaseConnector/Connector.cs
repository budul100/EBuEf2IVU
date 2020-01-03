using Common.Interfaces;
using Common.Models;
using DatabaseConnector.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DatabaseConnector
{
    public class Connector
        : IConnector
    {
        #region Private Fields

        private readonly CancellationToken cancellationToken;
        private readonly string connectionString;
        private readonly ILogger logger;
        private readonly AsyncRetryPolicy retryPolicy;

        #endregion Private Fields

        #region Public Constructors

        public Connector(ILogger logger, string connectionString, int retryTime, CancellationToken cancellationToken)
        {
            this.logger = logger;
            this.connectionString = connectionString;
            this.cancellationToken = cancellationToken;

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

        public Task AddRealtimeAsync(TrainPosition position)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (t) => SetTrainPositionsAsync(
                    position: position,
                    cancellationToken: t),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<DateTime> GetSessionDateAsync()
        {
            var result = retryPolicy.ExecuteAsync(
                action: (t) => GetSessionDateAsync(t),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<IEnumerable<VehicleAllocation>> GetVehicleAllocationsAsync()
        {
            var result = retryPolicy.ExecuteAsync(
                action: (t) => GetVehicleAllocationsAsync(t),
                cancellationToken: cancellationToken);

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private IEnumerable<string> GetFahrzeuge(Aufstellung aufstellung)
        {
            yield return aufstellung.Decoder;
        }

        private DateTime GetSessionDate(CancellationToken cancellationToken)
        {
            var result = DateTime.Today;

            if (!cancellationToken.IsCancellationRequested)
            {
                result = DateTime.Today;
            }

            return result;
        }

        private Task<DateTime> GetSessionDateAsync(CancellationToken cancellationToken)
        {
            return Task.Run<DateTime>(() => GetSessionDate(cancellationToken));
        }

        private IEnumerable<VehicleAllocation> GetVehicleAllocations(IEnumerable<Aufstellung> aufstellungen)
        {
            if (aufstellungen.Any())
            {
                foreach (var aufstellung in aufstellungen)
                {
                    logger.LogDebug($"Zug in Grundaufstellung gefunden: {aufstellung.ToString()}");

                    yield return new VehicleAllocation
                    {
                        Betriebsstelle = aufstellung.Feld.Betriebsstelle,
                        Fahrzeuge = GetFahrzeuge(aufstellung).ToArray(),
                        Gleis = aufstellung.Feld.Gleis,
                        Zugnummer = aufstellung.Zugnummer.ToString(),
                    };
                }
            }
            else
            {
                logger.LogInformation($"In der Grundaufstellung sind keine Fahrzeuge eingetragen.");
            }
        }

        private async Task<IEnumerable<VehicleAllocation>> GetVehicleAllocationsAsync(CancellationToken cancellationToken)
        {
            var result = Enumerable.Empty<VehicleAllocation>();

            if (!cancellationToken.IsCancellationRequested)
            {
                using (var context = new AufstellungenContext(connectionString))
                {
                    logger.LogDebug($"Suche nach allen Fahrzeugen der Grundaufstellung.");

                    var aufstellungen = await context.Aufstellungen
                        .Include(a => a.Feld.AbschnittZuFeld.Abschnitt)
                        .ToArrayAsync();

                    result = GetVehicleAllocations(aufstellungen).ToArray();
                }
            }

            return result;
        }

        private void OnRetry(Exception exception, TimeSpan reconnection)
        {
            logger.LogError($"Fehler beim Verbinden mit der EBuEf-Datenbank: {exception.Message}\r\n" +
                $"Die Verbindung wird in {reconnection.TotalSeconds} Sekunden wieder versucht.");
        }

        private async Task SaveTrainPositionAsync(TrainPosition position, bool istVon)
        {
            using (var context = new HalteContext(connectionString))
            {
                var betriebsstelle = istVon
                    ? position.EBuEfBetriebsstelleVon
                    : position.EBuEfBetriebsstelleNach;

                if (!string.IsNullOrWhiteSpace(betriebsstelle))
                {
                    var halt = context.Halte
                        .Where(h => h.Betriebsstelle == betriebsstelle)
                        .Include(h => h.Zug)
                        .FirstOrDefault(h => h.Zug.Zugnummer.ToString() == position.Zugnummer);

                    if (halt != null)
                    {
                        logger.LogDebug($@"Schreibe {(istVon ? "Von" : "Nach")}-Position in Session-Fahrplan.");

                        if (!string.IsNullOrWhiteSpace(istVon ? position.EBuEfGleisVon : position.EBuEfGleisNach))
                        {
                            halt.GleisIst = istVon ? position.EBuEfGleisVon : position.EBuEfGleisNach;
                        }

                        if (istVon)
                        {
                            halt.AbfahrtIst = position.EBuEfZeitpunktVon;
                        }
                        else
                        {
                            halt.AnkunftIst = position.EBuEfZeitpunktNach;
                        }

                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        logger.LogDebug($"In Session-Fahrplan wurde {(istVon ? "Von" : "Nach")}-Position nicht gefunden.");
                    }
                }
            }
        }

        private async Task SetTrainPositionsAsync(TrainPosition position, CancellationToken cancellationToken)
        {
            if (position != null && !cancellationToken.IsCancellationRequested)
            {
                logger.LogDebug($"Suche in Session-Fahrplan nach: {position.ToString()}");

                await SaveTrainPositionAsync(
                    position: position,
                    istVon: true);

                await SaveTrainPositionAsync(
                    position: position,
                    istVon: false);
            }
        }

        #endregion Private Methods
    }
}