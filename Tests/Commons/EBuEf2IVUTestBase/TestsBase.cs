﻿using Common.Enums;
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

        protected static Mock<IDatabaseConnector> GetDatabaseConnectorMock(Action sessionCallback = default,
            Action trainRunsPlanCallback = default, DateTime? ivuDatum = default)
        {
            var session = new EBuEfSession
            {
                IVUDatum = ivuDatum ?? DateTime.Today,
                SessionStart = DateTime.Now.TimeOfDay,
                Status = StateType.IsRunning,
            };

            var trainRunsMock = new List<TrainRun>();

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