using Common.Enums;
using Common.Interfaces;
using Common.Models;
using ConverterExtensions;
using DatabaseConnector.Contexts;
using DatabaseConnector.Extensions;
using DatabaseConnector.Models;
using EnumerableExtensions;
using Epoch.net;
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

        public Task AddRealtimeAsync(TrainLeg leg)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (token) => SaveTrainPositionsAsync(
                    leg: leg,
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

        public Task<TrainRun> GetTrainRunAsync(string trainNumber)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (token) => GetTrainRunAsync(
                    trainNumber: trainNumber,
                    cancellationToken: token),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<IEnumerable<TrainRun>> GetTrainRunsAsync(TimeSpan minTime, TimeSpan maxTime)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (token) => GetTrainRunsAsync(
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
                action: (token) => SaveCrewingsAsync(
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
                logger.LogDebug($"Suche in der EBuEf-DB nach der aktuellen Fahrplan-Session.");

                using var context = new SitzungenContext(connectionString);

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

        private IEnumerable<TrainPosition> GetTrainPositions(IEnumerable<Halt> halte)
        {
            if (halte.AnyItem())
            {
                foreach (var halt in halte)
                {
                    var result = new TrainPosition
                    {
                        Abfahrt = halt.AbfahrtPlan,
                        Ankunft = halt.AnkunftPlan,
                        Bemerkungen = halt.Bemerkungen,
                        Betriebsstelle = halt.Betriebsstelle,
                        Gleis = halt.GleisPlan.ToString(),
                        IstDurchfahrt = halt.IstDurchfahrt,
                    };

                    yield return result;
                }
            }
        }

        private TrainRun GetTrainRun(IEnumerable<Halt> halte)
        {
            var relevant = halte
                .OrderBy(h => h.SortierZeit).First();

            var positions = GetTrainPositions(halte).ToArray();

            var result = new TrainRun
            {
                Abfahrt = relevant.GetAbfahrt(),
                Bemerkungen = relevant.Zug.Bemerkungen,
                Positions = positions,
                Zugnummer = relevant.Zug?.Zugnummer.ToString(),
            };

            return result;
        }

        private async Task<TrainRun> GetTrainRunAsync(string trainNumber, CancellationToken cancellationToken)
        {
            var result = default(TrainRun);

            if (!cancellationToken.IsCancellationRequested)
            {
                logger.LogDebug(
                    message: "Suche nach Zug {trainNumber}.",
                    args: trainNumber);

                using var context = new HalteContext(connectionString);

                var zugnummer = trainNumber.ToInt();

                var halte = await context.Halte
                    .Include(h => h.Zug)
                    .Where(h => h.Zug.Zugnummer == zugnummer)
                    .ToArrayAsync(cancellationToken);

                result = GetTrainRun(halte);
            }

            return result;
        }

        private IEnumerable<TrainRun> GetTrainRuns(IEnumerable<Halt> halte)
        {
            if (halte.AnyItem())
            {
                var halteGroups = halte
                    .GroupBy(h => h.ZugID).ToArray();

                foreach (var halteGroup in halteGroups)
                {
                    var result = GetTrainRun(halteGroup);

                    yield return result;
                }
            }
        }

        private async Task<IEnumerable<TrainRun>> GetTrainRunsAsync(TimeSpan minTime, TimeSpan maxTime,
            CancellationToken cancellationToken)
        {
            var result = Enumerable.Empty<TrainRun>();

            if (!cancellationToken.IsCancellationRequested)
            {
                logger.LogDebug($"Suche nach aktuellen Fahrplaneinträgen.");

                using var context = new HalteContext(connectionString);

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

                    logger.LogDebug($"Zug in Grundaufstellung gefunden: {relevantAufstellung}");

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
                logger.LogDebug($"Suche nach allen Fahrzeugen der Grundaufstellung.");

                using var context = new AufstellungenContext(connectionString);

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

        private async Task SaveCrewingsAsync(IEnumerable<CrewingElement> crewingElements, CancellationToken cancellationToken)
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

        private async Task SaveTrainPositionAsync(TrainLeg leg, bool istVon, CancellationToken cancellationToken)
        {
            var betriebsstelle = istVon
                ? leg.EBuEfBetriebsstelleVon
                : leg.EBuEfBetriebsstelleNach;

            if (!string.IsNullOrWhiteSpace(betriebsstelle))
            {
                logger.LogDebug($"Suche in der EBuEf-DB nach dem letzten Halt von " +
                    $"Zug {leg.Zugnummer} in {betriebsstelle}.");

                using var context = new HalteContext(connectionString);

                var halt = context.Halte
                    .Where(h => h.Betriebsstelle == betriebsstelle)
                    .Include(h => h.Zug)
                    .FirstOrDefault(h => h.Zug.Zugnummer.ToString() == leg.Zugnummer);

                if (halt != null)
                {
                    logger.LogDebug($@"Schreibe {(istVon ? "Von" : "Nach")}-Position in Session-Fahrplan.");

                    if (!string.IsNullOrWhiteSpace(istVon ? leg.EBuEfGleisVon : leg.EBuEfGleisNach))
                    {
                        halt.GleisIst = istVon
                            ? leg.EBuEfGleisVon.ToInt()
                            : leg.EBuEfGleisNach.ToInt();
                    }

                    if (istVon)
                    {
                        halt.AbfahrtIst = leg.EBuEfZeitpunktVon;
                    }
                    else
                    {
                        halt.AnkunftIst = leg.EBuEfZeitpunktNach;
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

        private async Task SaveTrainPositionsAsync(TrainLeg leg, CancellationToken cancellationToken)
        {
            if (leg != null && !cancellationToken.IsCancellationRequested)
            {
                logger.LogDebug($"Suche in Session-Fahrplan nach: {leg}");

                await SaveTrainPositionAsync(
                    leg: leg,
                    istVon: true,
                    cancellationToken: cancellationToken);

                await SaveTrainPositionAsync(
                    leg: leg,
                    istVon: false,
                    cancellationToken: cancellationToken);
            }
        }

        #endregion Private Methods
    }
}