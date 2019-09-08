using Common.BusinessObjects;
using Common.Interfaces;
using EBuEfDBConnector.Models;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EBuEfDBConnector
{
    public class DataManager : IDataManager
    {
        #region Private Fields

        private readonly CancellationToken cancellationToken;
        private readonly ILogger logger;
        private string connectionString;
        private AsyncRetryPolicy retryPolicy;

        #endregion Private Fields

        #region Public Constructors

        public DataManager(ILogger logger, CancellationToken cancellationToken)
        {
            this.logger = logger;
            this.cancellationToken = cancellationToken;
        }

        #endregion Public Constructors

        #region Public Methods

        public Task<IEnumerable<VehicleAllocation>> GetVehicleAllocationsAsync()
        {
            return retryPolicy.ExecuteAsync(
                action: (t) => Task.Run<IEnumerable<VehicleAllocation>>(() => GetVehicleAllocations(t).ToArray()),
                cancellationToken: cancellationToken);
        }

        public void Run(string connectionString, int retryTime)
        {
            this.connectionString = connectionString;

            retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    sleepDurationProvider: (p) => TimeSpan.FromSeconds(retryTime),
                    onRetry: (exception, reconnection) => OnRetry(
                        exception: exception,
                        reconnection: reconnection));
        }

        public void SetTrainPosition(TrainPosition position)
        {
            retryPolicy.ExecuteAsync(
                action: (t) => Task.Run(() => SetTrainPosition(
                    position: position,
                    cancellationToken: t)),
                cancellationToken: cancellationToken);
        }

        #endregion Public Methods

        #region Private Methods

        private IEnumerable<string> GetFahrzeuge(Aufstellung aufstellung)
        {
            yield return aufstellung.Decoder;
        }

        private IEnumerable<VehicleAllocation> GetVehicleAllocations(CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                using (var context = new AufstellungenContext(connectionString))
                {
                    logger.Debug($"Suche nach allen Fahrzeugen der Grundaufstellung.");

                    var aufstellungen = context.Aufstellungen
                        .Include(a => a.Feld.AbschnittZuFeld.Abschnitt)
                        .ToArrayAsync().Result;

                    if (context.Aufstellungen.Any())
                    {
                        foreach (var aufstellung in context.Aufstellungen)
                        {
                            logger.Debug($"Zug in Grundaufstellung gefunden: {aufstellung.ToString()}");

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
                        logger.Information($"In der Grundaufstellung sind keine Fahrzeuge eingetragen.");
                    }
                }
            }
        }

        private void OnRetry(Exception exception, TimeSpan reconnection)
        {
            logger.Error($"Fehler beim Verbinden mit der EBuEf-Datenbank: {exception.Message}\r\n" +
                $"Die Verbindung wird in {reconnection.TotalSeconds} Sekunden wieder versucht.");
        }

        private void SaveTrainPosition(TrainPosition position, bool istVon)
        {
            using (var context = new HalteContext(connectionString))
            {
                var halt = context.Halte
                    .Where(h => h.Betriebsstelle == (istVon ? position.EBuEfBetriebsstelleVon : position.EBuEfBetriebsstelleNach))
                    .Include(h => h.Zug)
                    .FirstOrDefault(h => h.Zug.Zugnummer.ToString() == position.Zugnummer);

                if (halt != null)
                {
                    logger.Debug($@"Schreibe {(istVon ? "Von" : "Nach")}-Position in Session-Fahrplan.");

                    if (!string.IsNullOrWhiteSpace(istVon ? position.EBuEfGleisVon : position.EBuEfGleisNach))
                    {
                        halt.GleisIst = istVon ? position.EBuEfGleisVon : position.EBuEfGleisNach;
                    }

                    halt.AbfahrtIst = istVon ? position.EBuEfZeitpunktVon : position.EBuEfZeitpunktNach;

                    context.SaveChanges();
                }
                else
                {
                    logger.Debug($"In Session-Fahrplan wurde {(istVon ? "Von" : "Nach")}-Position nicht gefunden.");
                }
            }
        }

        private void SetTrainPosition(TrainPosition position, CancellationToken cancellationToken)
        {
            if (position != null && !cancellationToken.IsCancellationRequested)
            {
                logger.Debug($"Suche in Session-Fahrplan nach: {position.ToString()}");

                SaveTrainPosition(
                    position: position,
                    istVon: true);

                SaveTrainPosition(
                    position: position,
                    istVon: false);
            }
        }

        #endregion Private Methods
    }
}