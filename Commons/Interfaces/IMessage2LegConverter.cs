using Common.Models;
using System;

namespace Message2LegConverter
{
    public interface IMessage2LegConverter
    {
        #region Public Methods

        TrainLeg Convert(RealTimeMessage message);

        void Initialize(DateTime sessionDate);

        #endregion Public Methods
    }
}