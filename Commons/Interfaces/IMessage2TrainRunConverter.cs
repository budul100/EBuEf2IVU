using Commons.Models;
using System.Collections.Generic;

namespace Commons.Interfaces
{
    public interface IMessage2TrainRunConverter
    {
        #region Public Methods

        IEnumerable<TrainRun> Convert(IEnumerable<TrainPathMessage> messages);

        #endregion Public Methods
    }
}