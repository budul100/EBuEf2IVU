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
using StringExtensions;
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

        private string connectionString;
        private AsyncRetryPolicy retryPolicy;
        private CancellationToken sessionCancellationToken;

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
                action: (queryCancellationToken) => SaveTrainPositionsAsync(
                    leg: leg,
                    queryCancellationToken: queryCancellationToken),
                cancellationToken: sessionCancellationToken);

            return result;
        }

        public Task<EBuEfSession> GetEBuEfSessionAsync()
        {
            var result = retryPolicy.ExecuteAsync(
                action: (queryCancellationToken) => QuerySessionDateAsync(queryCancellationToken),
                cancellationToken: sessionCancellationToken);

            return result;
        }

        public Task<IEnumerable<string>> GetLocationShortnamesAsync(IEnumerable<string> locationTypes)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (queryCancellationToken) => QueryLocationShortnames(
                    locationTypes: locationTypes,
                    queryCancellationToken: queryCancellationToken),
                cancellationToken: sessionCancellationToken);

            return result;
        }

        public Task<IEnumerable<TrainRun>> GetTrainRunsAsync(string trainId, bool preferPrognosis = false)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (queryCancellationToken) => QueryTrainRunsAsync(
                    trainId: trainId,
                    preferPrognosis: preferPrognosis,
                    queryCancellationToken: queryCancellationToken),
                cancellationToken: sessionCancellationToken);

            return result;
        }

        public Task<IEnumerable<TrainRun>> GetTrainRunsAsync(bool preferPrognosis = false)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (queryCancellationToken) => QueryTrainRunsAsync(
                    trainId: default,
                    preferPrognosis: preferPrognosis,
                    queryCancellationToken: queryCancellationToken),
                cancellationToken: sessionCancellationToken);

            return result;
        }

        public Task<IEnumerable<TrainRun>> GetTrainRunsAsync(TimeSpan minTime, TimeSpan maxTime)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (queryCancellationToken) => QueryTrainRunsAsync(
                    minTime: minTime,
                    maxTime: maxTime,
                    queryCancellationToken: queryCancellationToken),
                cancellationToken: sessionCancellationToken);

            return result;
        }

        public Task<int?> GetTrainTypeIdAsync(string zuggattung)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (queryCancellationToken) => QueryTrainTypeId(
                    zuggattung: zuggattung,
                    queryCancellationToken: queryCancellationToken),
                cancellationToken: sessionCancellationToken);

            return result;
        }

        public Task<IEnumerable<VehicleAllocation>> GetVehicleAllocationsAsync()
        {
            var result = retryPolicy.ExecuteAsync(
                action: (queryCancellationToken) => QueryVehicleAllocationsAsync(queryCancellationToken),
                cancellationToken: sessionCancellationToken);

            return result;
        }

        public void Initialize(string connectionString, int retryTime, CancellationToken sessionCancellationToken)
        {
            this.connectionString = connectionString;
            this.sessionCancellationToken = sessionCancellationToken;

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
                action: (queryCancellationToken) => SaveCrewingsAsync(
                    crewingElements: crewingElements,
                    queryCancellationToken: queryCancellationToken),
                cancellationToken: sessionCancellationToken);

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
                    var trainId = QueryTrainId(crewingElement.Zugnummer);

                    if (trainId.HasValue)
                    {
                        var predecessorTrainId = QueryTrainId(crewingElement.ZugnummerVorgaenger);

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

        private IEnumerable<TrainPosition> GetTrainPositions(IEnumerable<Halt> halte, bool preferPrognosis)
        {
            if (halte.AnyItem())
            {
                foreach (var halt in halte)
                {
                    var result = new TrainPosition
                    {
                        Abfahrt = halt.GetAbfahrtPath(preferPrognosis),
                        Ankunft = halt.GetAnkunftPath(preferPrognosis),
                        Bemerkungen = halt.Bemerkungen,
                        Betriebsstelle = halt.Betriebsstelle,
                        Gleis = halt.GleisPlan.ToString(),
                        IstDurchfahrt = halt.IstDurchfahrt,
                    };

                    yield return result;
                }
            }
        }

        private IEnumerable<TrainRun> GetTrainRuns(IEnumerable<Halt> halte, bool preferPrognosis)
        {
            if (halte.AnyItem())
            {
                var halteGroups = halte
                    .GroupBy(h => h.ZugID).ToArray();

                foreach (var halteGroup in halteGroups)
                {
                    var ordered = halteGroup
                        .OrderBy(h => h.SortierZeit).ToArray();

                    var positions = GetTrainPositions(
                        halte: ordered,
                        preferPrognosis: preferPrognosis).ToArray();

                    var relevant = ordered.First();

                    var result = new TrainRun
                    {
                        Abfahrt = relevant.GetAbfahrt(),
                        Bemerkungen = relevant.Zug.Bemerkungen,
                        Positions = positions,
                        Zuggattung = relevant.Zug.Zuggattung.Kurzname,
                        ZugId = relevant.ZugID,
                        Zugnummer = relevant.Zug.Zugnummer,
                    };

                    yield return result;
                }
            }
        }

        private IEnumerable<VehicleAllocation> GetVehicleAllocations(IEnumerable<Aufstellung> aufstellungen)
        {
            if (aufstellungen.Any())
            {
                var aufstellungenGroups = aufstellungen
                    .GroupBy(a => new
                    {
                        a.Feld.Betriebsstelle,
                        a.Feld.Gleis,
                        a.Decoder,
                        a.Zugnummer
                    }).ToArray();

                logger.LogDebug(
                    "Es wurden {numberTrains} Züge in der Grundaufstellungen gefunden.",
                    aufstellungenGroups.Count());

                foreach (var aufstellungenGroup in aufstellungenGroups)
                {
                    var relevantAufstellung = aufstellungenGroup.First();

                    var fahrzeuge = relevantAufstellung.Decoder.ToString().AsEnumerable();

                    var result = new VehicleAllocation
                    {
                        Betriebsstelle = relevantAufstellung.Feld?.Betriebsstelle,
                        Fahrzeuge = fahrzeuge,
                        Gleis = relevantAufstellung.Feld?.Gleis.ToString(),
                        Zugnummer = relevantAufstellung.Zugnummer?.ToString(),
                    };

                    logger.LogDebug(
                        "Zug in Grundaufstellung: {train}",
                        relevantAufstellung);

                    yield return result;
                }
            }
            else
            {
                logger.LogInformation(
                    "In der Grundaufstellung sind keine Fahrzeuge eingetragen.");
            }
        }

        private void OnRetry(Exception exception, TimeSpan reconnection)
        {
            while (exception.InnerException != null) exception = exception.InnerException;

            logger.LogError(
                "Fehler beim Verbinden mit der EBuEf-Datenbank: {message}\r\n" +
                "Die Verbindung wird in {reconnection} Sekunden wieder versucht.",
                exception.Message,
                reconnection.TotalSeconds);
        }

        private async Task<IEnumerable<string>> QueryLocationShortnames(IEnumerable<string> locationTypes,
            CancellationToken queryCancellationToken)
        {
            using var context = new BetriebsstelleContext(connectionString);

            var betriebsstellen = await context.Betriebsstellen
                .ToArrayAsync(queryCancellationToken);

            var result = betriebsstellen?
                .Where(b => !locationTypes.AnyItem() || locationTypes.Contains(b.Art))
                .Select(b => b.Kurzname).ToArray();

            return result;
        }

        private async Task<EBuEfSession> QuerySessionDateAsync(CancellationToken queryCancellationToken)
        {
            var result = default(EBuEfSession);

            if (!queryCancellationToken.IsCancellationRequested)
            {
                logger.LogDebug(
                    "Suche in der EBuEf-DB nach der aktuellen Fahrplan-Session.");

                using var context = new SitzungContext(connectionString);

                var sitzung = await context.Sitzungen
                    .Where(s => s.Status == Convert.ToByte(SessionStates.InPreparation)
                        || s.Status == Convert.ToByte(SessionStates.IsRunning)
                        || s.Status == Convert.ToByte(SessionStates.IsPaused))
                    .OrderByDescending(s => s.Status == Convert.ToByte(SessionStates.IsRunning))
                    .ThenByDescending(s => s.Status == Convert.ToByte(SessionStates.IsPaused))
                    .FirstOrDefaultAsync(queryCancellationToken);

                if (sitzung != default)
                {
                    var timeshift = new TimeSpan(
                        hours: 0,
                        minutes: 0,
                        seconds: sitzung.Verschiebung * -1);

                    result = new EBuEfSession
                    {
                        IVUDatum = sitzung.IvuDate ?? DateTime.Today,
                        SessionKey = sitzung.SessionKey,
                        SessionStart = sitzung.SimulationStartzeit.ToDateTime().ToLocalTime(),
                        Verschiebung = timeshift,
                    };

                    logger.LogDebug(
                        "Aktuelle EBuEf-Sitzung gefunden: {session}",
                        result);
                }
                else
                {
                    logger.LogError(
                        "Es wurde keine EBuEf-Session gefunden.");
                }
            }

            return result;
        }

        private int? QueryTrainId(string zugnummer)
        {
            var result = default(int?);

            if (!zugnummer.IsEmpty())
            {
                using var context = new ZugContext(connectionString);

                var trainNumber = zugnummer.ToInt();

                result = context.Zuege
                    .FirstOrDefault(z => z.Zugnummer == trainNumber)?.ID;
            }

            return result;
        }

        private async Task<IEnumerable<TrainRun>> QueryTrainRunsAsync(string trainId, bool preferPrognosis,
            CancellationToken queryCancellationToken)
        {
            var result = Enumerable.Empty<TrainRun>();

            if (!queryCancellationToken.IsCancellationRequested)
            {
                if (!trainId.IsEmpty())
                {
                    logger.LogDebug(
                        "Suche nach Zug-ID {trainId}.",
                        trainId);
                }
                else
                {
                    logger.LogDebug(
                        "Suche nach allen Zügen.");
                }

                using var context = new HaltContext(connectionString);

                var zugId = trainId.ToNullableInt();

                var halte = await context.Halte
                    .Include(h => h.Zug).ThenInclude(z => z.Zuggattung)
                    .Where(h => !zugId.HasValue || h.ZugID == zugId)
                    .ToArrayAsync(queryCancellationToken);

                result = GetTrainRuns(
                    halte: halte,
                    preferPrognosis: preferPrognosis).ToArray();
            }

            return result;
        }

        private async Task<IEnumerable<TrainRun>> QueryTrainRunsAsync(TimeSpan minTime, TimeSpan maxTime,
            CancellationToken queryCancellationToken)
        {
            var result = Enumerable.Empty<TrainRun>();

            if (!queryCancellationToken.IsCancellationRequested)
            {
                logger.LogDebug(
                    "Suche nach aktuellen Fahrplaneinträgen.");

                using var context = new HaltContext(connectionString);

                var halte = await context.Halte
                    .Include(h => h.Zug).ThenInclude(z => z.Zuggattung)
                    .Where(h => h.AbfahrtIst.HasValue || h.AbfahrtSoll.HasValue || h.AbfahrtPlan.HasValue)
                    .Where(h => (h.AbfahrtIst ?? h.AbfahrtSoll ?? h.AbfahrtPlan) >= minTime)
                    .Where(h => (h.AbfahrtIst ?? h.AbfahrtSoll ?? h.AbfahrtPlan) <= maxTime)
                    .ToArrayAsync(queryCancellationToken);

                result = GetTrainRuns(
                    halte: halte,
                    preferPrognosis: false).ToArray();
            }

            return result;
        }

        private async Task<int?> QueryTrainTypeId(string zuggattung, CancellationToken queryCancellationToken)
        {
            var result = default(Zuggattung);

            if (!zuggattung.IsEmpty())
            {
                using var context = new ZugGattungContext(connectionString);

                result = await context.Zuggattungen.FirstOrDefaultAsync(
                    predicate: g => g.Kurzname == zuggattung,
                    cancellationToken: queryCancellationToken);
            }

            return result?.ID;
        }

        private async Task<IEnumerable<VehicleAllocation>> QueryVehicleAllocationsAsync(CancellationToken queryCancellationToken)
        {
            var result = Enumerable.Empty<VehicleAllocation>();

            if (!queryCancellationToken.IsCancellationRequested)
            {
                logger.LogDebug(
                    "Suche nach allen Fahrzeugen der Grundaufstellung.");

                using var context = new AufstellungContext(connectionString);

                var aufstellungen = await context.Aufstellungen
                    .Include(a => a.Feld)
                    .ThenInclude(f => f.AbschnittZuFeld)
                    .ThenInclude(z => z.Abschnitt).ToArrayAsync(queryCancellationToken);

                result = GetVehicleAllocations(aufstellungen).ToArray();
            }

            return result;
        }

        private async Task SaveCrewingsAsync(IEnumerable<CrewingElement> crewingElements, CancellationToken queryCancellationToken)
        {
            if (crewingElements?.Any() ?? false)
            {
                using var context = new BesatzungContext(connectionString);

                context.Database.OpenConnection();

                var besatzungen = GetCrewings(crewingElements).ToArray();

                if (besatzungen.Any())
                {
                    logger.LogDebug(
                        "Die vorhandenen Crewing-Einträge in der EBuEf-DB werden gelöscht.");

                    context.Besatzungen.RemoveRange(context.Besatzungen);

                    logger.LogDebug(
                        "Es werden {crewsNumber} Crewing-Einträge in der EBuEf-DB gespeichert.",
                        besatzungen.Count());

                    context.Besatzungen.AddRange(besatzungen);

                    await context.SaveChangesAsync(queryCancellationToken);

                    logger.LogDebug(
                        "Die Crewing-Einträge wurden gespeichert.");
                }

                context.Database.CloseConnection();
            }
        }

        private async Task SaveTrainPositionAsync(TrainLeg leg, bool istVon, CancellationToken queryCancellationToken)
        {
            var betriebsstelle = istVon
                ? leg.EBuEfBetriebsstelleVon
                : leg.EBuEfBetriebsstelleNach;

            if (!string.IsNullOrWhiteSpace(betriebsstelle))
            {
                logger.LogDebug(
                    "Suche in der EBuEf-DB nach dem letzten Halt von Zug {train} in {location}.",
                    leg.Zugnummer,
                    betriebsstelle);

                using var context = new HaltContext(connectionString);

                var halt = context.Halte
                    .Where(h => h.Betriebsstelle == betriebsstelle)
                    .Include(h => h.Zug)
                    .FirstOrDefault(h => h.Zug.Zugnummer.ToString() == leg.Zugnummer);

                if (halt != null)
                {
                    logger.LogDebug(
                        "Schreibe {positionType}-Position in Session-Fahrplan.",
                        istVon.GetName());

                    halt.GleisIst = leg.GetEBuEfGleis(istVon);

                    if (istVon)
                    {
                        halt.AbfahrtIst = leg.EBuEfZeitpunktVon;
                    }
                    else
                    {
                        halt.AnkunftIst = leg.EBuEfZeitpunktNach;
                    }

                    await context.SaveChangesAsync(queryCancellationToken);

                    logger.LogDebug(
                        "Das Update des Halts wurde gespeichert.");
                }
                else
                {
                    logger.LogDebug(
                        "In Session-Fahrplan wurde {positionType}-Position nicht gefunden.",
                        istVon.GetName());
                }
            }
        }

        private async Task SaveTrainPositionsAsync(TrainLeg leg, CancellationToken queryCancellationToken)
        {
            if (leg != null && !queryCancellationToken.IsCancellationRequested)
            {
                logger.LogDebug(
                    "Suche in Session-Fahrplan nach: {leg}",
                    leg);

                await SaveTrainPositionAsync(
                    leg: leg,
                    istVon: true,
                    queryCancellationToken: queryCancellationToken);

                await SaveTrainPositionAsync(
                    leg: leg,
                    istVon: false,
                    queryCancellationToken: queryCancellationToken);
            }
        }

        #endregion Private Methods
    }
}