using Common.Enums;
using Common.EventsArgs;
using Common.Interfaces;
using Common.Models;
using EBuEf2IVUBase.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EBuEf2IVUBase
{
    public abstract class WorkerBase
        : BackgroundService
    {
        #region Protected Fields

        protected static readonly TimeSpan delay = TimeSpan.FromMilliseconds(10);

        protected readonly IConfiguration config;
        protected readonly IDatabaseConnector databaseConnector;
        protected readonly ILogger logger;
        protected readonly IStateHandler sessionStateHandler;

        protected EBuEfSession ebuefSession;
        protected DateTime ebuefSessionStart = DateTime.Now;
        protected DateTime ivuSessionDate = DateTime.Now;

        #endregion Protected Fields

        #region Private Fields

        private CancellationTokenSource sessionCancellationTokenSource;

        #endregion Private Fields

        #region Protected Constructors

        protected WorkerBase(IConfiguration config, IStateHandler sessionStateHandler,
            IDatabaseConnector databaseConnector, ILogger logger, Assembly assembly)
        {
            var assemblyInfo = assembly.GetName();
            logger.LogInformation(
                "{name} (Version {version}) wird gestartet.",
                assemblyInfo.Name,
                $"{assemblyInfo.Version.Major}.{assemblyInfo.Version.Minor}");

            this.config = config;
            this.logger = logger;

            this.sessionStateHandler = sessionStateHandler;
            this.sessionStateHandler.SessionChangedEvent += OnSessionChanged;

            this.databaseConnector = databaseConnector;
        }

        #endregion Protected Constructors

        #region Protected Methods

        protected CancellationToken GetSessionCancellationToken(CancellationToken workerCancellationToken)
        {
            sessionCancellationTokenSource = new CancellationTokenSource();
            workerCancellationToken.Register(() => sessionCancellationTokenSource.Cancel());

            return sessionCancellationTokenSource.Token;
        }

        protected async Task InitializeConnectionAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Die Datenbank-Verbindungen und der Session-State-Empfänger werden gestartet.");

            var databaseConnectionSettings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            databaseConnector.Initialize(
                connectionString: databaseConnectionSettings.ConnectionString,
                retryTime: databaseConnectionSettings.RetryTime,
                cancellationToken: cancellationToken);

            var statucReceiverSettings = config
                .GetSection(nameof(StatusReceiver))
                .Get<StatusReceiver>();

            sessionStateHandler.Initialize(
                host: statucReceiverSettings.Host,
                port: statucReceiverSettings.Port,
                retryTime: statucReceiverSettings.RetryTime,
                startPattern: statucReceiverSettings.StartPattern,
                statusPattern: statucReceiverSettings.StatusPattern);

            await sessionStateHandler.ExecuteAsync(cancellationToken);
        }

        protected async Task InitializeSessionAsync()
        {
            if (databaseConnector == default)
            {
                throw new ApplicationException(
                    "Der Datenbank-Connector muss zuerst initialisiert werden.");
            }

            ebuefSession = await databaseConnector.GetEBuEfSessionAsync();

            if (ebuefSession != default)
            {
                ivuSessionDate = ebuefSession.IVUDatum;
                ebuefSessionStart = ivuSessionDate
                    .Add(ebuefSession.SessionStart.TimeOfDay);

                logger.LogDebug(
                    "Die IVU-Sitzung läuft am {sessionDate} um {sessionTime}.",
                    ivuSessionDate.ToString("yyyy-MM-dd"),
                    ebuefSessionStart.ToString("HH:mm"));
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void OnSessionChanged(object sender, StateChangedArgs e)
        {
            if (e.StateType == StateType.IsEnded)
            {
                sessionCancellationTokenSource?.Cancel();
            }
        }

        #endregion Private Methods
    }
}