using EBuEf2IVU.Shareds.Commons.Models;
using System.Collections.Generic;

namespace EBuEf2IVU.Shareds.Commons.Interfaces
{
    public interface IMessage2TrainRunConverter
    {
        #region Public Methods

        IEnumerable<TrainRun> Convert(IEnumerable<TrainPathMessage> messages);

        #endregion Public Methods
    }
}