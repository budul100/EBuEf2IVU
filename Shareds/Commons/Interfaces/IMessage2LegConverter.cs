using EBuEf2IVU.Shareds.Commons.Models;

namespace EBuEf2IVU.Shareds.Commons.Interfaces
{
    public interface IMessage2LegConverter
    {
        #region Public Methods

        TrainLeg Convert(RealTimeMessage message);

        #endregion Public Methods
    }
}