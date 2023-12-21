using Commons.Models;

namespace Commons.Interfaces
{
    public interface IMessage2LegConverter
    {
        #region Public Methods

        TrainLeg Convert(RealTimeMessage message);

        #endregion Public Methods
    }
}