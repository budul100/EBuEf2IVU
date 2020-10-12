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
                action: (token) => QuerySessionDateAsync(token),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<IEnumerable<TrainRun>> GetTrainRunsAsync(string trainNumber, bool preferPrognosis)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (token) => QueryTrainRunsAsync(
                    trainNumber: trainNumber,
                    preferPrognosis: preferPrognosis,
                    cancellationToken: token),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<IEnumerable<TrainRun>> GetTrainRunsAsync(bool preferPrognosis)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (token) => QueryTrainRunsAsync(
                    trainNumber: default,
                    preferPrognosis: preferPrognosis,
                    cancellationToken: token),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<IEnumerable<TrainRun>> GetTrainRunsAsync(TimeSpan minTime, TimeSpan maxTime)
        {
            var result = retryPolicy.ExecuteAsync(
                action: (token) => QueryTrainRunsAsync(
                    minTime: minTime,
                    maxTime: maxTime,
                    cancellationToken: token),
                cancellationToken: cancellationToken);

            return result;
        }

        public Task<IEnumerable<VehicleAllocation>> GetVehicleAllocationsAsync()
        {
            var result = retryPolicy.ExecuteAsync(
                action: (token) => QueryVehicleAllocationsAsync(token),
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
                        Zugnummer = relevant.Zug?.Zugnummer.ToString(),
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

        private async Task<EBuEfSession> QuerySessionDateAsync(CancellationToken cancellationToken)
        {
            var result = default(EBuEfSession);

            if (!cancellationToken.IsCancellationRequested)
            {
                logger.LogDebug(
                    "Suche in der EBuEf-DB nach der aktuellen Fahrplan-Session.");

                using var context = new SitzungenContext(connectionString);

                var sitzung = await context.Sitzungen
                    .Where(s => s.HasRelevantStatus())
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
                using var context = new ZuegeContext(connectionString);

                var trainNumber = zugnummer.ToInt();

                result = context.Zuege
                    .FirstOrDefault(z => z.Zugnummer == trainNumber)?.ID;
            }

            return result;
        }

        private async Task<IEnumerable<TrainRun>> QueryTrainRunsAsync(string trainNumber, bool preferPrognosis,
            CancellationToken cancellationToken)
        {
            var result = Enumerable.Empty<TrainRun>();

            if (!cancellationToken.IsCancellationRequested)
            {
                logger.LogDebug(
                    "Suche nach Zugnummer {trainNumber}.",
                    trainNumber);

                using var context = new HalteContext(connectionString);

                var zugnummer = trainNumber.ToNullableInt();

                var halte = await context.Halte
                    .Include(h => h.Zug)
                    .Where(h => !zugnummer.HasValue || h.Zug.Zugnummer == zugnummer)
                    .ToArrayAsync(cancellationToken);

                result = GetTrainRuns(
                    halte: halte,
                    preferPrognosis: preferPrognosis).ToArray();
            }

            return result;
        }

        private async Task<IEnumerable<TrainRun>> QueryTrainRunsAsync(TimeSpan minTime, TimeSpan maxTime,
            CancellationToken cancellationToken)
        {
            var result = Enumerable.Empty<TrainRun>();

            if (!cancellationToken.IsCancellationRequested)
            {
                logger.LogDebug(
                    "Suche nach aktuellen Fahrplaneinträgen.");

                using var context = new HalteContext(connectionString);

                var halte = await context.Halte
                    .Include(h => h.Zug)
                    .Where(h => h.HasAbfahrt())
                    .Where(h => h.GetAbfahrt() >= minTime)
                    .Where(h => h.GetAbfahrt() <= maxTime)
                    .ToArrayAsync(cancellationToken);

                result = GetTrainRuns(
                    halte: halte,
                    preferPrognosis: false).ToArray();
            }

            return result;
        }

        private async Task<IEnumerable<VehicleAllocation>> QueryVehicleAllocationsAsync(CancellationToken cancellationToken)
        {
            var result = Enumerable.Empty<VehicleAllocation>();

            if (!cancellationToken.IsCancellationRequested)
            {
                logger.LogDebug(
                    "Suche nach allen Fahrzeugen der Grundaufstellung.");

                using var context = new AufstellungenContext(connectionString);

                var aufstellungen = await context.Aufstellungen
                    .Include(a => a.Feld)
                    .ThenInclude(f => f.AbschnittZuFeld)
                    .ThenInclude(z => z.Abschnitt).ToArrayAsync(cancellationToken);

                result = GetVehicleAllocations(aufstellungen).ToArray();
            }

            return result;
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
                    logger.LogDebug(
                        "Die vorhandenen Crewing-Einträge in der EBuEf-DB werden gelöscht.");

                    context.Besatzungen.RemoveRange(context.Besatzungen);

                    logger.LogDebug(
                        "Es werden {crewsNumber} Crewing-Einträge in der EBuEf-DB gespeichert.",
                        besatzungen.Count());

                    context.Besatzungen.AddRange(besatzungen);

                    await context.SaveChangesAsync(cancellationToken);

                    logger.LogDebug(
                        "Die Crewing-Einträge wurden gespeichert.");
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
                logger.LogDebug(
                    "Suche in der EBuEf-DB nach dem letzten Halt von Zug {train} in {location}.",
                    leg.Zugnummer,
                    betriebsstelle);

                using var context = new HalteContext(connectionString);

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

                    await context.SaveChangesAsync(cancellationToken);

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

        private async Task SaveTrainPositionsAsync(TrainLeg leg, CancellationToken cancellationToken)
        {
            if (leg != null && !cancellationToken.IsCancellationRequested)
            {
                logger.LogDebug(
                    "Suche in Session-Fahrplan nach: {leg}",
                    leg);

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