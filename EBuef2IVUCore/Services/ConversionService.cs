using Common.Interfaces;
using Common.Settings;
using EBuEf2IVUCore.Converters;
using System;

namespace EBuEf2IVUCore.Services
{
    internal class ConversionService
    {
        #region Private Fields

        private IDataManager dataManager;
        private ISenderManager senderManager;
        private TrainPositionConverter trainPositionConverter;

        #endregion Private Fields

        #region Public Methods

        public void Run(IReceiverManager receiverManager, IDataManager dataManager, ISenderManager senderManager, EBuEf2IVUSettings settings)
        {
            this.dataManager = dataManager;
            this.senderManager = senderManager;

            receiverManager.RealTimeReceivedEvent += OnRealTimeReceived;
            receiverManager.SessionStartedEvent += OnSessionStarted;

            trainPositionConverter = new TrainPositionConverter(
                settings.InfrastructureMappings,
                settings.SessionDateIVU);

            dataManager.Run(
                connectionString: settings.DatabaseConnectionString,
                retryTime: settings.DatabaseRetryTime);

            senderManager.Run(
                retryTime: settings.SenderRetryTime,
                ivuEndpoint: settings.SenderEndpoint,
                ivuDivision: settings.SenderDivision);

            receiverManager.Run(
                allocationsIpAdress: settings.AllocationsIpAddress,
                allocationsListenerPort: settings.AllocationsListenerPort,
                allocationsRetryTime: settings.AllocationsRetryTime,
                allocationsPattern: settings.AllocationsPattern,
                messagesIpAdress: settings.MessagesIpAddress,
                messagesListenerPort: settings.MessagesListenerPort,
                messagesRetryTime: settings.MessagesRetryTime);
        }

        #endregion Public Methods

        #region Private Methods

        private void OnRealTimeReceived(object sender, Common.EventsArgs.RealTimeReceivedArgs e)
        {
            var position = trainPositionConverter.GetTrainPosition(e.RealTimeMessage);

            if (position != null)
            {
                dataManager.AddRealtime(position);
                senderManager.AddRealtime(position);
            }
        }

        private async void OnSessionStarted(object sender, EventArgs e)
        {
            var allocations = await dataManager.GetVehicleAllocationsAsync();
            senderManager.AddAllocations(allocations);
        }

        #endregion Private Methods
    }
}