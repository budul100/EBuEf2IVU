using Common.Interfaces;
using DatabaseConnector;
using EBuEf2IVUVehicle.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EBuEf2IVUCrew
{
    internal class Worker
        : BackgroundService
    {
        #region Private Fields

        private readonly IConfiguration config;
        private readonly ILogger<Worker> logger;
        private IConnector databaseConnector;
        private CancellationTokenSource sessionCancellationTokenSource;

        #endregion Private Fields

        #region Public Constructors

        public Worker(IConfiguration config, ILogger<Worker> logger)
        {
            this.config = config;
            this.logger = logger;
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override async Task ExecuteAsync(CancellationToken workerCancellationToken)
        {
            while (!workerCancellationToken.IsCancellationRequested)
            {
                var sessionCancellationToken = GetSessionCancellationToken(workerCancellationToken);
                databaseConnector = GetConnector(sessionCancellationToken);

                await TestAsync();
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private IConnector GetConnector(CancellationToken sessionCancellationToken)
        {
            var settings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            logger.LogDebug($"Verbindung zur Datenbank herstellen: {settings.ConnectionString}.");

            var result = new Connector(
                logger: logger,
                connectionString: settings.ConnectionString,
                retryTime: settings.RetryTime,
                cancellationToken: sessionCancellationToken);

            return result;
        }

        private CancellationToken GetSessionCancellationToken(CancellationToken workerCancellationToken)
        {
            sessionCancellationTokenSource = new CancellationTokenSource();
            workerCancellationToken.Register(() => sessionCancellationTokenSource.Cancel());

            return sessionCancellationTokenSource.Token;
        }

        private async Task TestAsync()
        {
        }

        #endregion Private Methods
    }
}