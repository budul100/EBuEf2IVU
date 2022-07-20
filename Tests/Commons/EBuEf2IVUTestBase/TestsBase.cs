using Common.Enums;
using Common.Interfaces;
using Common.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EBuEf2IVUTestBase
{
    public abstract class TestsBase
    {
        #region Protected Methods

        protected static Mock<IDatabaseConnector> GetDatabaseConnectorMock(Action getEbuefSession = default)
        {
            var session = new EBuEfSession
            {
                IVUDatum = DateTime.Today,
                SessionStart = DateTime.Now,
                Status = StateType.IsRunning,
            };

            var trainRunsMock = new List<TrainRun> { Mock.Of<TrainRun>() };

            var result = new Mock<IDatabaseConnector>();

            result
                .Setup(m => m.GetEBuEfSessionActiveAsync())
                .Returns(Task.FromResult(true));

            result
                .Setup(m => m.GetEBuEfSessionAsync())
                .Callback(() => getEbuefSession?.Invoke())
                .Returns(Task.FromResult(session));

            result
                .Setup(m => m.GetTrainRunsDispoAsync(
                    It.IsAny<TimeSpan>(),
                    It.IsAny<TimeSpan>()))
                .Returns(Task.FromResult((IEnumerable<TrainRun>)trainRunsMock));

            return result;
        }

        protected static Mock<IStateHandler> GetStateHandlerMock(StateType sessionStatusType = StateType.IsEnded)
        {
            var result = new Mock<IStateHandler>();

            result
                .SetupGet(m => m.StateType)
                .Returns(sessionStatusType);

            return result;
        }

        #endregion Protected Methods
    }
}