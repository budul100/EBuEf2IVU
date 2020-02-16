using Common.Interfaces;
using Common.Models;
using DatabaseConnector.Contexts;
using DatabaseConnector.Extensions;
using DatabaseConnector.Models;
using Extensions;
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
        #region Public Fields

        public const byte SessionInPreparation = 1;
        public const byte SessionIsPaused = 5;
        public const byte SessionIsRunning = 2;

        #endregion Public Fields

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
                    onRetry: (exception, reconnection) => OnRetry(
                        exception: exception,
                        reconnection: reconnection),
                    sleepDurationProvider: (p) => TimeSpan.FromSeconds(retryTime));
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

        public Task<EBuEfSession> GetEBuEfSessionAsync()
        {
            var result = retryPolicy.ExecuteAsync(
                action: (t) => GetSessionDateAsync(t),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<IEnumerable<TimetableStop>> GetTimetableStops()
        {
            var result = retryPolicy.ExecuteAsync(
                action: (t) => GetTimetableStops(t),
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
            yield return aufstellung.Decoder.ToString();
        }

        private async Task<EBuEfSession> GetSessionDateAsync(CancellationToken cancellationToken)
        {
            var result = default(EBuEfSession);

            if (!cancellationToken.IsCancellationRequested)
            {
                using var context = new SitzungContext(connectionString);

                logger.LogDebug($"Suche in der EBuEf-DB nach der aktuellen Fahrplan-Session.");

                var sitzung = await context.Sitzungen
                    .Where(s => s.Status == SessionInPreparation
                        || s.Status == SessionIsRunning
                        || s.Status == SessionIsPaused)
                    .OrderByDescending(s => s.Status == SessionIsRunning)
                    .ThenByDescending(s => s.Status == SessionIsPaused)
                    .FirstOrDefaultAsync(cancellationToken);

                if (sitzung != default)
                {
                    result = new EBuEfSession
                    {
                        IVUDate = sitzung.IvuDate ?? DateTime.Today,
                        SessionStart = sitzung.SimulationStartzeit.ToDateTime(),
                    };

                    logger.LogDebug($"Aktuelle EBuEf-Sitzung gefunden: {result}");
                }
                else
                {
                    logger.LogError($"Es wurde keine EBuEf-Session gefunden.");
                }
            }

            return result;
        }

        private async Task<IEnumerable<TimetableStop>> GetTimetableStops(CancellationToken cancellationToken)
        {
            var result = Enumerable.Empty<TimetableStop>();

            if (!cancellationToken.IsCancellationRequested)
            {
                using var context = new HalteContext(connectionString);

                logger.LogDebug($"Suche nach aktuellen Fahrplaneinträgen.");

                var halte = await context.Halte
                    .Include(h => h.Zug)
                    .ToArrayAsync(cancellationToken);

                result = GetTimetableStops(halte).ToArray();
            }

            return result;
        }

        private IEnumerable<TimetableStop> GetTimetableStops(IEnumerable<Halt> halte)
        {
            if (halte.Any())
            {
                foreach (var halt in halte)
                {
                    var result = new TimetableStop
                    {
                        Abfahrt = halt.AbfahrtPlan,
                        Ankunft = halt.AnkunftPlan,
                        Betriebsstelle = halt.Betriebsstelle,
                        Zugnummer = halt.Zug?.Zugnummer.ToString(),
                    };

                    yield return result;
                }
            }
            else
            {
                logger.LogInformation($"In der Grundaufstellung sind keine Fahrzeuge eingetragen.");
            }
        }

        private IEnumerable<VehicleAllocation> GetVehicleAllocations(IEnumerable<Aufstellung> aufstellungen)
        {
            if (aufstellungen.Any())
            {
                var aufstellungenGroups = aufstellungen
                    .GroupBy(a => new { a.Feld.Betriebsstelle, a.Feld.Gleis, a.Decoder, a.Zugnummer }).ToArray();

                logger.LogDebug($"Es wurden {aufstellungenGroups.Count()} Züge in der Grundaufstellungen gefunden.");

                foreach (var aufstellungenGroup in aufstellungenGroups)
                {
                    var relevantAufstellung = aufstellungenGroup.First();

                    var result = new VehicleAllocation
                    {
                        Betriebsstelle = relevantAufstellung.Feld?.Betriebsstelle,
                        Fahrzeuge = GetFahrzeuge(relevantAufstellung).ToArray(),
                        Gleis = relevantAufstellung.Feld?.Gleis.ToString(),
                        Zugnummer = relevantAufstellung.Zugnummer?.ToString(),
                    };

                    logger.LogDebug($"Zug in Grundaufstellung gefunden: {relevantAufstellung.ToString()}");

                    yield return result;
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
                using var context = new AufstellungenContext(connectionString);

                logger.LogDebug($"Suche nach allen Fahrzeugen der Grundaufstellung.");

                var aufstellungen = await context.Aufstellungen
                    .Include(a => a.Feld)
                    .ThenInclude(f => f.AbschnittZuFeld)
                    .ThenInclude(z => z.Abschnitt).ToArrayAsync(cancellationToken);

                result = GetVehicleAllocations(aufstellungen).ToArray();
            }

            return result;
        }

        private void OnRetry(Exception exception, TimeSpan reconnection)
        {
            logger.LogError($"Fehler beim Verbinden mit der EBuEf-Datenbank: {exception.Message}\r\n" +
                $"Die Verbindung wird in {reconnection.TotalSeconds} Sekunden wieder versucht.");
        }

        private async Task SaveTrainPositionAsync(TrainPosition position, bool istVon, CancellationToken cancellationToken)
        {
            using var context = new HalteContext(connectionString);

            var betriebsstelle = istVon
                ? position.EBuEfBetriebsstelleVon
                : position.EBuEfBetriebsstelleNach;

            if (!string.IsNullOrWhiteSpace(betriebsstelle))
            {
                logger.LogDebug($"Suche in der EBuEf-DB nach dem letzten Halt von " +
                    $"Zug {position.Zugnummer} in {betriebsstelle}.");

                var halt = context.Halte
                    .Where(h => h.Betriebsstelle == betriebsstelle)
                    .Include(h => h.Zug)
                    .FirstOrDefault(h => h.Zug.Zugnummer.ToString() == position.Zugnummer);

                if (halt != null)
                {
                    logger.LogDebug($@"Schreibe {(istVon ? "Von" : "Nach")}-Position in Session-Fahrplan.");

                    if (!string.IsNullOrWhiteSpace(istVon ? position.EBuEfGleisVon : position.EBuEfGleisNach))
                    {
                        halt.GleisIst = istVon
                            ? position.EBuEfGleisVon.ToInt()
                            : position.EBuEfGleisNach.ToInt();
                    }

                    if (istVon)
                    {
                        halt.AbfahrtIst = position.EBuEfZeitpunktVon;
                    }
                    else
                    {
                        halt.AnkunftIst = position.EBuEfZeitpunktNach;
                    }

                    await context.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    logger.LogDebug($"In Session-Fahrplan wurde {(istVon ? "Von" : "Nach")}-Position nicht gefunden.");
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
                    istVon: true,
                    cancellationToken: cancellationToken);

                await SaveTrainPositionAsync(
                    position: position,
                    istVon: false,
                    cancellationToken: cancellationToken);
            }
        }

        #endregion Private Methods
    }
}