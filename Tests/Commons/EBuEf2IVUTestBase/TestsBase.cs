using Commons.Enums;
using Commons.Interfaces;
using Commons.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EBuEf2IVUTestBase
{
    public abstract class TestsBase
    {
        #region Protected Methods

        protected static Mock<IDatabaseConnector> GetDatabaseConnectorMock(bool sendTrainRuns = false,
            Action sessionCallback = default, Action trainRunsPlanCallback = default,
            Action<TrainLeg> addRealtimeCallback = default, DateTime? ivuDatum = default)
        {
            var session = new EBuEfSession
            {
                IVUDatum = ivuDatum ?? DateTime.Today,
                SessionStart = DateTime.Now.TimeOfDay,
                Status = StateType.IsRunning,
            };

            var trainRunsMock = new List<TrainRun>();

            if (sendTrainRuns)
            {
                trainRunsMock.Add((new Mock<TrainRun>()).Object);
            }

            var result = new Mock<IDatabaseConnector>();

            result
                .Setup(m => m.GetEBuEfSessionActiveAsync())
                .Returns(Task.FromResult(true));

            result
                .Setup(m => m.GetEBuEfSessionAsync())
                .Callback(() => sessionCallback?.Invoke())
                .Returns(Task.FromResult(session));

            result
                .Setup(m => m.GetTrainRunsPlanAsync(
                    It.IsAny<int>(),
                    It.IsAny<DayOfWeek>(),
                    It.IsAny<bool>()))
                .Callback(() => trainRunsPlanCallback?.Invoke())
                .Returns(Task.FromResult((IEnumerable<TrainRun>)trainRunsMock));

            result
                .Setup(m => m.GetTrainRunsDispoAsync(
                    It.IsAny<TimeSpan>(),
                    It.IsAny<TimeSpan>()))
                .Returns(Task.FromResult((IEnumerable<TrainRun>)trainRunsMock));

            result
                .Setup(m => m.AddRealtimeAsync(
                    It.IsAny<TrainLeg>()))
                .Callback<TrainLeg>(l => addRealtimeCallback?.Invoke(l));

            return result;
        }

        protected static Mock<ILogger<T>> GetLoggerMock<T>(Action errorCallback = default)
        {
            var result = new Mock<ILogger<T>>();

            result
                .Setup(x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<SocketException>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()))
                .Callback(() => errorCallback?.Invoke());

            result
                .Setup(x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<EntryPointNotFoundException>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()))
                .Callback(() => errorCallback?.Invoke());

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