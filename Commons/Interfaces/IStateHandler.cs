﻿using Commons.Enums;
using Commons.EventsArgs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Commons.Interfaces
{
    public interface IStateHandler
    {
        #region Public Events

        event EventHandler<StateChangedArgs> SessionChangedEvent;

        #endregion Public Events

        #region Public Properties

        StateType StateType { get; }

        #endregion Public Properties

        #region Public Methods

        Task ExecuteAsync(CancellationToken cancellationToken);

        void Initialize(string host, int port, int retryTime, string startPattern, string statusPattern);

        void Initialize(string server, string topic, int retryTime, string startPattern,
            string statusPattern);

        #endregion Public Methods
    }
}