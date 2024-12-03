using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Commons.Enums;
using Commons.EventsArgs;
using Commons.Extensions;
using Commons.Interfaces;
using Commons.Models;
using Commons.Settings;

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
        protected DateTime ivuDatum = DateTime.Today;
        protected DateTime sessionStart = DateTime.Now;

        #endregion Protected Fields

        #region Private Fields

        private CancellationTokenSource sessionCancellationTokenSource;

        #endregion Private Fields

        #region Protected Constructors

        protected WorkerBase(IConfiguration config, IStateHandler sessionStateHandler,
            IDatabaseConnector databaseConnector, ILogger logger, Assembly assembly)
        {
            var assemblyInfo = assembly.GetName();

            var version = $"{assemblyInfo.Version.Major}.{assemblyInfo.Version.Minor}.{assemblyInfo.Version.Build}";

            logger.LogInformation(
                "{name} (Version {version}) wird gestartet.",
                assemblyInfo.Name,
                version);

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

        protected abstract Task HandleSessionStateAsync(StateType stateType);

        protected async Task InitializeConnectionAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Die Datenbank-Verbindungen und der Session-State-Empfänger werden gestartet.");

            var connectionSettings = config
                .GetSection(nameof(EBuEfDBConnector))
                .Get<EBuEfDBConnector>();

            var connectionString = connectionSettings.GetDBConnectionString();

            databaseConnector.Initialize(
                connectionString: connectionSettings.ConnectionString,
                retryTime: connectionSettings.RetryTime,
                cancellationToken: cancellationToken);

            var receiverSettings = config
                .GetSection(nameof(StatusReceiver))
                .Get<StatusReceiver>();

            var useMulticast = receiverSettings.UseMulticast(StatusReceiver.EnvironmentFormat);

            if (useMulticast)
            {
                var host = receiverSettings.GetMCHost(StatusReceiver.EnvironmentMCHost);
                var port = receiverSettings.GetMCPort(
                    variableName: StatusReceiver.EnvironmentMCPort,
                    defaultPort: StatusReceiver.MulticastPortDefault);

                sessionStateHandler.Initialize(
                    host: host,
                    port: port,
                    retryTime: receiverSettings.RetryTime,
                    startPattern: receiverSettings.StartPattern,
                    statusPattern: receiverSettings.StatusPattern);
            }
            else
            {
                var host = receiverSettings.GetMQTTHost(StatusReceiver.EnvironmentMQTTHost);
                var port = receiverSettings.GetMQTTPort(StatusReceiver.EnvironmentMQTTPort);
                var topic = receiverSettings.GetMQTTTopic(StatusReceiver.EnvironmentMQTTTopic);

                sessionStateHandler.Initialize(
                    server: host,
                    port: port,
                    topic: topic,
                    retryTime: receiverSettings.RetryTime,
                    startPattern: receiverSettings.StartPattern,
                    statusPattern: receiverSettings.StatusPattern);
            }

            await sessionStateHandler.ExecuteAsync(cancellationToken);
        }

        protected virtual async Task InitializeSessionAsync()
        {
            if (databaseConnector == default)
            {
                throw new ApplicationException(
                    "Der Datenbank-Connector muss zuerst initialisiert werden.");
            }

            ebuefSession = await databaseConnector.GetEBuEfSessionAsync();

            if (ebuefSession != default)
            {
                ivuDatum = ebuefSession.IVUDatum;
                sessionStart = ivuDatum.Add(ebuefSession.SessionStart);

                logger.LogDebug(
                    "Die IVU-Sitzung läuft am {sessionDate} um {sessionTime}.",
                    ivuDatum.ToString("yyyy-MM-dd"),
                    sessionStart.ToString("HH:mm"));
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private async void OnSessionChanged(object sender, StateChangedArgs e)
        {
            if (e.StateType == StateType.IsEnded)
            {
                sessionCancellationTokenSource?.Cancel();
            }

            await HandleSessionStateAsync(e.StateType);
        }

        #endregion Private Methods
    }
}