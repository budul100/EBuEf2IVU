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

        private const int IdSunday = 6;

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
                action: (queryCancellationToken) => SaveTrainPositionsAsync(
                    leg: leg,
                    queryCancellationToken: queryCancellationToken),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<bool> GetEBuEfSessionActiveAsync()
        {
            var result = retryPolicy.ExecuteAsync(
                action: (queryCancellationToken) => QueryEBuEfSessionActiveAsync(queryCancellationToken),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<EBuEfSession> GetEBuEfSessionAsync()
        {
            var result = retryPolicy.ExecuteAsync(
                action: (queryCancellationToken) => QueryEBuEfSessionAsync(queryCancellationToken),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<IEnumerable<string>> GetLocationShortnamesAsync(IEnumerable<string> locationTypes)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (queryCancellationToken) => QueryLocationShortnames(
                    locationTypes: locationTypes,
                    queryCancellationToken: queryCancellationToken),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<IEnumerable<TrainRun>> GetTrainRunsDispoAsync(int trainId, bool preferPrognosis = false)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (queryCancellationToken) => QueryTrainRunsDispoAsync(
                    trainId: trainId,
                    preferPrognosis: preferPrognosis,
                    queryCancellationToken: queryCancellationToken),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<IEnumerable<TrainRun>> GetTrainRunsDispoAsync(TimeSpan minTime, TimeSpan maxTime)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (queryCancellationToken) => QueryTrainRunsDispoAsync(
                    minTime: minTime,
                    maxTime: maxTime,
                    queryCancellationToken: queryCancellationToken),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<IEnumerable<TrainRun>> GetTrainRunsPlanAsync(int timetableId, DayOfWeek weekday,
            bool preferPrognosis = false)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (queryCancellationToken) => QueryTrainRunsPlanAsync(
                    timetableId: timetableId,
                    weekday: weekday,
                    preferPrognosis: preferPrognosis,
                    queryCancellationToken: queryCancellationToken),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<int?> GetTrainTypeIdAsync(string zuggattung)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (queryCancellationToken) => QueryTrainTypeId(
                    zuggattung: zuggattung,
                    queryCancellationToken: queryCancellationToken),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<IEnumerable<VehicleAllocation>> GetVehicleAllocationsAsync()
        {
            var result = retryPolicy.ExecuteAsync(
                action: (queryCancellationToken) => QueryVehicleAllocationsAsync(queryCancellationToken),
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
                action: (queryCancellationToken) => SaveCrewingsAsync(
                    crewingElements: crewingElements,
                    queryCancellationToken: queryCancellationToken),
                cancellationToken: cancellationToken);

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private async Task<IEnumerable<Besatzung>> GetCrewingsAsync(IEnumerable<CrewingElement> crewingElements,
            CancellationToken queryCancellationToken)
        {
            var result = new List<Besatzung>();

            if (crewingElements?.Any() ?? false)
            {
                foreach (var crewingElement in crewingElements)
                {
                    queryCancellationToken.ThrowIfCancellationRequested();

                    var trainId = await QueryTrainIdAsync(
                        zugnummer: crewingElement.Zugnummer,
                        queryCancellationToken: queryCancellationToken);

                    if (trainId.HasValue)
                    {
                        var predecessorTrainId = await QueryTrainIdAsync(
                            zugnummer: crewingElement.ZugnummerVorgaenger,
                            queryCancellationToken: queryCancellationToken);

                        var besatzung = new Besatzung
                        {
                            BetriebsstelleNach = crewingElement.BetriebsstelleNach,
                            BetriebsstelleVon = crewingElement.BetriebsstelleVon,
                            Dienst = crewingElement.DienstKurzname,
                            PersonalNachname = crewingElement.PersonalNachname,
                            PersonalNummer = crewingElement.PersonalNummer.ToNullableInt(),
                            VorgaengerZugId = predecessorTrainId,
                            ZugId = trainId.Value,
                        };

                        result.Add(besatzung);
                    }
                }
            }

            return result;
        }

        private IEnumerable<TrainPosition> GetTrainPositions<T>(IEnumerable<Halt<T>> halte, bool preferPrognosis)
            where T : Zug
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
                        VerkehrNicht = false,
                    };

                    yield return result;
                }
            }
        }

        private IEnumerable<TrainRun> GetTrainRuns<T>(IEnumerable<Halt<T>> halte, bool preferPrognosis)
            where T : Zug
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

        private async Task<bool> QueryEBuEfSessionActiveAsync(CancellationToken queryCancellationToken)
        {
            var result = false;

            if (!queryCancellationToken.IsCancellationRequested)
            {
                logger.LogDebug(
                    "Suche in der EBuEf-DB nach dem Status der aktuellen Fahrplan-Session.");

                using var context = new SitzungContext(connectionString);

                result = await context.Sitzungen
                    .Where(s => s.Status == Convert.ToByte(SessionStatusType.IsRunning))
                    .AnyAsync(queryCancellationToken);
            }

            return result;
        }

        private async Task<EBuEfSession> QueryEBuEfSessionAsync(CancellationToken queryCancellationToken)
        {
            var result = default(EBuEfSession);

            if (!queryCancellationToken.IsCancellationRequested)
            {
                logger.LogDebug(
                    "Suche in der EBuEf-DB nach der aktuellen Fahrplan-Session.");

                using var context = new SitzungContext(connectionString);

                var sitzung = await context.Sitzungen
                    .Where(s => s.Status == Convert.ToByte(SessionStatusType.InPreparation)
                        || s.Status == Convert.ToByte(SessionStatusType.IsRunning)
                        || s.Status == Convert.ToByte(SessionStatusType.IsPaused))
                    .OrderByDescending(s => s.Status == Convert.ToByte(SessionStatusType.IsRunning))
                    .ThenByDescending(s => s.Status == Convert.ToByte(SessionStatusType.IsPaused))
                    .FirstOrDefaultAsync(queryCancellationToken);

                if (sitzung != default)
                {
                    var timeshift = new TimeSpan(
                        hours: 0,
                        minutes: 0,
                        seconds: sitzung.Verschiebung * -1);

                    result = new EBuEfSession
                    {
                        FahrplanId = sitzung.FahrplanId,
                        IVUDatum = sitzung.IvuDate ?? DateTime.Today,
                        SessionKey = sitzung.SessionKey,
                        SessionStart = sitzung.SimStartzeit.ToDateTime().ToLocalTime(),
                        Verschiebung = timeshift,
                        Wochentag = sitzung.SimWochentag.GetWochentag(),
                    };

                    logger.LogDebug(
                        "Aktuelle Fahrplan-Session gefunden: {session}",
                        result);
                }
                else
                {
                    logger.LogError(
                        "Es wurde keine Fahrplan-Session gefunden.");
                }
            }

            return result;
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

        private async Task<int?> QueryTrainIdAsync(string zugnummer, CancellationToken queryCancellationToken)
        {
            var result = default(int?);

            if (!zugnummer.IsEmpty())
            {
                using var context = new ZugDispoContext(connectionString);

                var trainNumber = zugnummer.ToInt();

                var train = await context.Zuege
                    .Where(z => z.Zugnummer == trainNumber)
                    .FirstOrDefaultAsync(queryCancellationToken);

                result = train?.ID;
            }

            return result;
        }

        private async Task<IEnumerable<TrainRun>> QueryTrainRunsDispoAsync(TimeSpan minTime, TimeSpan maxTime,
            CancellationToken queryCancellationToken)
        {
            var result = Enumerable.Empty<TrainRun>();

            if (!queryCancellationToken.IsCancellationRequested)
            {
                logger.LogDebug(
                    "Suche nach aktuellen Fahrplaneinträgen.");

                using var context = new HaltDispoContext(connectionString);

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

        private async Task<IEnumerable<TrainRun>> QueryTrainRunsDispoAsync(int trainId, bool preferPrognosis,
            CancellationToken queryCancellationToken)
        {
            var result = Enumerable.Empty<TrainRun>();

            if (!queryCancellationToken.IsCancellationRequested)
            {
                logger.LogDebug(
                    "Suche nach Zug-ID {trainId}.",
                    trainId);

                using var context = new HaltDispoContext(connectionString);

                var halte = await context.Halte
                    .Include(h => h.Zug).ThenInclude(z => z.Zuggattung)
                    .Where(h => h.ZugID == trainId)
                    .ToArrayAsync(queryCancellationToken);

                result = GetTrainRuns(
                    halte: halte,
                    preferPrognosis: preferPrognosis).ToArray();
            }

            return result;
        }

        private async Task<IEnumerable<TrainRun>> QueryTrainRunsPlanAsync(int timetableId, DayOfWeek weekday,
            bool preferPrognosis, CancellationToken queryCancellationToken)
        {
            var result = Enumerable.Empty<TrainRun>();

            if (!queryCancellationToken.IsCancellationRequested)
            {
                logger.LogDebug(
                    "Suche nach allen Zügen.");

                using var context = new HaltPlanContext(connectionString);

                var weekdayId = weekday != DayOfWeek.Sunday
                    ? ((int)weekday) - 1
                    : IdSunday;

                var allHalte = await context.Halte
                    .Include(h => h.Zug).ThenInclude(z => z.Zuggattung)
                    .Where(h => h.Zug.FahrplanId == timetableId)
                    .Where(h => h.Zug.Bitmask.Length >= weekdayId)
                    .ToArrayAsync(queryCancellationToken);

                var currentHalte = allHalte
                    .Where(h => h.Zug.Bitmask[weekdayId] == QueryExtensions.PositiveBit).ToArray();

                result = GetTrainRuns(
                    halte: currentHalte,
                    preferPrognosis: preferPrognosis).ToArray();
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

                var besatzungen = await GetCrewingsAsync(crewingElements, queryCancellationToken);

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

                using var context = new HaltDispoContext(connectionString);

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