using Common.Enums;
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
        : IDatabaseConnector
    {
        #region Private Fields

        private readonly ILogger logger;
        private CancellationToken cancellationToken;
        private string connectionString;
        private AsyncRetryPolicy retryPolicy;

        #endregion Private Fields

        #region Public Constructors

        public Connector(ILogger<Connector> logger)
        {
            this.logger = logger;
        }

        #endregion Public Constructors

        #region Public Methods

        public Task AddRealtimeAsync(TrainPosition position)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (token) => SaveTrainPositionsAsync(
                    position: position,
                    cancellationToken: token),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<EBuEfSession> GetEBuEfSessionAsync()
        {
            var result = retryPolicy.ExecuteAsync(
                action: (token) => GetSessionDateAsync(token),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<IEnumerable<TrainRun>> GetTrainRunsAsync(TimeSpan minTime, TimeSpan maxTime)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (token) => GetTrainRuns(
                    minTime: minTime,
                    maxTime: maxTime,
                    cancellationToken: token),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<IEnumerable<VehicleAllocation>> GetVehicleAllocationsAsync()
        {
            var result = retryPolicy.ExecuteAsync(
                action: (token) => GetVehicleAllocationsAsync(token),
                cancellationToken: cancellationToken);

            return result;
        }

        public void Initialize(string connectionString, int retryTime, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Verwendete Datenbankverbindung: {connectionString}.");

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

        public Task SetCrewingsAsync(IEnumerable<CrewingElement> crewingElements)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (token) => SaveCrewings(
                    crewingElements: crewingElements,
                    cancellationToken: token),
                cancellationToken: cancellationToken);

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private IEnumerable<Besatzung> GetCrewings(IEnumerable<CrewingElement> crewingElements)
        {
            if (crewingElements?.Any() ?? false)
            {
                foreach (var crewingElement in crewingElements)
                {
                    var trainId = GetTrainId(crewingElement.Zugnummer);

                    if (trainId.HasValue)
                    {
                        var predecessorTrainId = GetTrainId(crewingElement.ZugnummerVorgaenger);

                        var result = new Besatzung
                        {
                            BetriebsstelleNach = crewingElement.BetriebsstelleNach,
                            BetriebsstelleVon = crewingElement.BetriebsstelleVon,
                            Dienst = crewingElement.DienstKurzname,
                            PersonalNachname = crewingElement.PersonalNachname,
                            PersonalNummer = crewingElement.PersonalNummer.ToNullableInt(),
                            VorgaengerZugId = predecessorTrainId,
                            ZugId = trainId.Value,
                        };

                        yield return result;
                    }
                }
            }
        }

        private async Task<EBuEfSession> GetSessionDateAsync(CancellationToken cancellationToken)
        {
            var result = default(EBuEfSession);

            if (!cancellationToken.IsCancellationRequested)
            {
                using var context = new SitzungenContext(connectionString);

                logger.LogDebug($"Suche in der EBuEf-DB nach der aktuellen Fahrplan-Session.");

                var sitzung = await context.Sitzungen
                    .Where(s => s.Status == Convert.ToByte(SessionStates.InPreparation)
                        || s.Status == Convert.ToByte(SessionStates.IsRunning)
                        || s.Status == Convert.ToByte(SessionStates.IsPaused))
                    .OrderByDescending(s => s.Status == Convert.ToByte(SessionStates.IsRunning))
                    .ThenByDescending(s => s.Status == Convert.ToByte(SessionStates.IsPaused))
                    .FirstOrDefaultAsync(cancellationToken);

                if (sitzung != default)
                {
                    var timeshift = new TimeSpan(
                        hours: 0,
                        minutes: 0,
                        seconds: sitzung.Verschiebung * -1);

                    result = new EBuEfSession
                    {
                        IVUDatum = sitzung.IvuDate ?? DateTime.Today,
                        SessionStart = sitzung.SimulationStartzeit.ToDateTime(),
                        Verschiebung = timeshift,
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

        private int? GetTrainId(string zugnummer)
        {
            var result = default(int?);

            if (!string.IsNullOrWhiteSpace(zugnummer))
            {
                using var context = new ZuegeContext(connectionString);

                var trainNumber = zugnummer.ToInt();

                result = context.Zuege
                    .FirstOrDefault(z => z.Zugnummer == trainNumber)?.ID;
            }

            return result;
        }

        private IEnumerable<TrainRun> GetTrainRuns(IEnumerable<Halt> halte)
        {
            if (halte.Any())
            {
                var relevants = halte
                    .GroupBy(h => h.ZugID)
                    .Select(g => g.OrderBy(h => h.GetAbfahrt()).First())
                    .Distinct().ToArray();

                foreach (var relevant in relevants)
                {
                    var result = new TrainRun
                    {
                        Abfahrt = relevant.GetAbfahrt(),
                        Zugnummer = relevant.Zug?.Zugnummer.ToString(),
                    };

                    yield return result;
                }
            }
        }

        private async Task<IEnumerable<TrainRun>> GetTrainRuns(TimeSpan minTime, TimeSpan maxTime,
            CancellationToken cancellationToken)
        {
            var result = Enumerable.Empty<TrainRun>();

            if (!cancellationToken.IsCancellationRequested)
            {
                using var context = new HalteContext(connectionString);

                logger.LogDebug($"Suche nach aktuellen Fahrplaneinträgen.");

                var halte = await context.Halte
                    .Include(h => h.Zug)
                    .Where(h => h.AbfahrtIst.HasValue || h.AbfahrtSoll.HasValue || h.AbfahrtPlan.HasValue)
                    .Where(h => (h.AbfahrtIst ?? h.AbfahrtSoll ?? h.AbfahrtPlan ?? TimeSpan.MinValue) >= minTime)
                    .Where(h => (h.AbfahrtSoll ?? h.AbfahrtPlan ?? TimeSpan.MaxValue) <= maxTime)
                    .ToArrayAsync(cancellationToken);

                result = GetTrainRuns(halte).ToArray();
            }

            return result;
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
                        Fahrzeuge = GetVehicles(relevantAufstellung).ToArray(),
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

        private IEnumerable<string> GetVehicles(Aufstellung aufstellung)
        {
            yield return aufstellung.Decoder.ToString();
        }

        private void OnRetry(Exception exception, TimeSpan reconnection)
        {
            while (exception.InnerException != null) exception = exception.InnerException;

            logger.LogError($"Fehler beim Verbinden mit der EBuEf-Datenbank: {exception.Message}\r\n" +
                $"Die Verbindung wird in {reconnection.TotalSeconds} Sekunden wieder versucht.");
        }

        private async Task SaveCrewings(IEnumerable<CrewingElement> crewingElements, CancellationToken cancellationToken)
        {
            if (crewingElements?.Any() ?? false)
            {
                using var context = new BesatzungenContext(connectionString);

                context.Database.OpenConnection();

                var besatzungen = GetCrewings(crewingElements).ToArray();

                if (besatzungen.Any())
                {
                    logger.LogDebug($"Die vorhandenen Crewing-Einträge in der EBuEf-DB werden gelöscht.");

                    context.Besatzungen.RemoveRange(context.Besatzungen);

                    logger.LogDebug($"Es werden {besatzungen.Count()} Crewing-Einträge in der EBuEf-DB gespeichert.");

                    context.Besatzungen.AddRange(besatzungen);

                    await context.SaveChangesAsync(cancellationToken);

                    logger.LogDebug("Die Crewing-Einträge wurden gespeichert.");
                }

                context.Database.CloseConnection();
            }
        }

        private async Task SaveTrainPositionAsync(TrainPosition position, bool istVon, CancellationToken cancellationToken)
        {
            var betriebsstelle = istVon
                ? position.EBuEfBetriebsstelleVon
                : position.EBuEfBetriebsstelleNach;

            if (!string.IsNullOrWhiteSpace(betriebsstelle))
            {
                using var context = new HalteContext(connectionString);

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

                    logger.LogDebug("Das Update des Halts wurde gespeichert.");
                }
                else
                {
                    logger.LogDebug($"In Session-Fahrplan wurde {(istVon ? "Von" : "Nach")}-Position nicht gefunden.");
                }
            }
        }

        private async Task SaveTrainPositionsAsync(TrainPosition position, CancellationToken cancellationToken)
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