using Common.Models;

namespace Common.Interfaces
{
    public interface IMessage2LegConverter
    {
        #region Public Methods

        TrainLeg Convert(RealTimeMessage message);

        #endregion Public Methods
    }
}