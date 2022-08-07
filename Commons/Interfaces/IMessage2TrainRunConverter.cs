using Common.Models;
using System;
using System.Collections.Generic;

namespace Common.Interfaces
{
    public interface IMessage2TrainRunConverter
    {
        #region Public Methods

        IEnumerable<TrainRun> Convert(IEnumerable<TrainPathMessage> messages);

        void Initialize(DateTime? ivuDatum, string sessionKey);

        #endregion Public Methods
    }
}