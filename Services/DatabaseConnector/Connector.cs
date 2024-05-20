using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Commons.Enums;
using Commons.Interfaces;
using Commons.Models;
using ConverterExtensions;
using DatabaseConnector.Contexts;
using DatabaseConnector.Extensions;
using DatabaseConnector.Models;
using EnumerableExtensions;
using Epoch.net;
using Polly;
using Polly.Retry;
using StringExtensions;

namespace DatabaseConnector
{
    public class Connector
        : IDatabaseConnector
    {
        #region Private Fields

        private const int IdSunday = 6;
        private const int SessionIdDefault = 0;

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

        public Task<IEnumerable<TrainRun>> GetTrainRunsPlanAsync(int timetableId, DayOfWeek weekday, bool preferPrognosis = false)
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

            logger.LogDebug(
                "Die Datenbank wird wie folgt aufgerufen: {connectionString}",
                connectionString);

            retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    sleepDurationProvider: _ => TimeSpan.FromSeconds(retryTime),
                    onRetry: (exception, reconnection) => OnRetry(
                        exception: exception,
                        reconnection: reconnection));
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

        private IEnumerable<VehicleAllocation> GetVehicleAllocations(IEnumerable<Aufstellung> aufstellungen)
        {
            if (aufstellungen.Any())
            {
                var aufstellungenGroups = aufstellungen
                    .GroupBy(a => (a.Feld.Betriebsstelle, a.Feld.Gleis, a.Decoder, a.Zugnummer)).ToArray();

                logger.LogDebug(
                    "Es wurden {numberTrains} Züge in der Grundaufstellungen gefunden.",
                    aufstellungenGroups.Length);

                foreach (var aufstellungenGroup in aufstellungenGroups)
                {
                    var relevant = aufstellungenGroup.First();
                    var fahrzeuge = relevant.Decoder.ToString().AsEnumerable();

                    var result = new VehicleAllocation
                    {
                        Betriebsstelle = relevant.Feld?.Betriebsstelle,
                        Fahrzeuge = fahrzeuge,
                        Gleis = relevant.Feld?.Gleis.ToString(),
                        Zugnummer = relevant.Zugnummer?.ToString(),
                    };

                    logger.LogDebug(
                        "Zug in Grundaufstellung: {train}",
                        relevant);

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
            while (exception.InnerException != default) exception = exception.InnerException;

            logger.LogError(
                exception,
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

                await using var context = new SitzungContext(connectionString);

                result = await context.Sitzungen
                    .Where(s => s.Status == Convert.ToByte(StateType.IsRunning))
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

                await using var context = new SitzungContext(connectionString);

                var sitzung = await context.Sitzungen
                    .Where(s => s.Id != SessionIdDefault
                        && (s.Status == Convert.ToByte(StateType.InPreparation)
                        || s.Status == Convert.ToByte(StateType.IsRunning)
                        || s.Status == Convert.ToByte(StateType.IsPaused)))
                    .OrderByDescending(s => s.Status == Convert.ToByte(StateType.IsRunning))
                    .ThenByDescending(s => s.Status == Convert.ToByte(StateType.IsPaused))
                    .FirstOrDefaultAsync(queryCancellationToken);

                if (sitzung != default)
                {
                    var timeshift = new TimeSpan(
                        hours: 0,
                        minutes: 0,
                        seconds: sitzung.Verschiebung);

                    var realStart = sitzung.RealStartzeit
                        .ToDateTime().ToLocalTime().TimeOfDay;
                    var sessionStart = sitzung.SimStartzeit
                        .ToDateTime().ToLocalTime().TimeOfDay;

                    result = new EBuEfSession
                    {
                        FahrplanId = sitzung.FahrplanId,
                        IVUDatum = sitzung.IvuDatum ?? DateTime.Today,
                        Name = sitzung.Name,
                        RealStart = realStart,
                        SessionKey = sitzung.SessionKey,
                        SessionStart = sessionStart,
                        Status = (StateType)sitzung.Status,
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
            await using var context = new FahrplanpunktContext(connectionString);

            var fahrplanpunkte = await context.Fahrplanpunkte
                .ToArrayAsync(queryCancellationToken);

            var result = fahrplanpunkte?
                .Where(b => !locationTypes.AnyItem() || locationTypes.Contains(b.Art))
                .Select(b => b.Kurzname).ToArray();

            return result;
        }

        private async Task<int?> QueryTrainIdAsync(string zugnummer, CancellationToken queryCancellationToken)
        {
            var result = default(int?);

            if (!zugnummer.IsEmpty())
            {
                await using var context = new ZugDispoContext(connectionString);

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

                await using var context = new HaltDispoContext(connectionString);

                var halte = await context.Halte
                    .Include(h => h.Zug).ThenInclude(z => z.Zuggattung)
                    .Where(h => h.AbfahrtIst.HasValue || h.AbfahrtSoll.HasValue || h.AbfahrtPlan.HasValue)
                    .Where(h => (h.AbfahrtIst ?? h.AbfahrtSoll ?? h.AbfahrtPlan) >= minTime)
                    .Where(h => (h.AbfahrtIst ?? h.AbfahrtSoll ?? h.AbfahrtPlan) <= maxTime)
                    .ToArrayAsync(queryCancellationToken);

                result = halte.GetTrainRuns(
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

                await using var context = new HaltDispoContext(connectionString);

                var halte = await context.Halte
                    .Include(h => h.Zug).ThenInclude(z => z.Zuggattung)
                    .Where(h => h.ZugID == trainId)
                    .ToArrayAsync(queryCancellationToken);

                result = halte.GetTrainRuns(
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

                await using var context = new HaltPlanContext(connectionString);

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

                result = currentHalte.GetTrainRuns(
                    preferPrognosis: preferPrognosis).ToArray();
            }

            return result;
        }

        private async Task<int?> QueryTrainTypeId(string zuggattung, CancellationToken queryCancellationToken)
        {
            var result = default(Zuggattung);

            if (!zuggattung.IsEmpty())
            {
                await using var context = new ZugGattungContext(connectionString);

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

                await using var context = new AufstellungContext(connectionString);

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
                await using var context = new BesatzungContext(connectionString);

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
            if (!leg.IstPrognose)
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

                    await using var context = new HaltDispoContext(connectionString);

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