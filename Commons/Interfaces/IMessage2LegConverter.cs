using Common.Models;
using System;

namespace Common.Interfaces
{
    public interface IMessage2LegConverter
    {
        #region Public Methods

        TrainLeg Convert(RealTimeMessage message);

        void Initialize(DateTime? ivuDatum);

        #endregion Public Methods
    }
}