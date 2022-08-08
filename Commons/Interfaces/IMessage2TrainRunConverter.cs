using Common.Models;
using System.Collections.Generic;

namespace Common.Interfaces
{
    public interface IMessage2TrainRunConverter
    {
        #region Public Methods

        IEnumerable<TrainRun> Convert(IEnumerable<TrainPathMessage> messages);

        #endregion Public Methods
    }
}