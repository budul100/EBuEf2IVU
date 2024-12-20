﻿using System;

namespace Commons.Settings
{
    public class RealtimeSender
        : ConnectorIVUBase
    {
        #region Public Fields

        public const string EnvironmentIVUIFEndpoint = "IVU_IFSERVER_ENDPOINT";

        #endregion Public Fields

        #region Public Properties

        public DateTime? DateMin { get; set; }

        public string Division { get; set; }

        public string Endpoint { get; set; }

        public bool IgnorePrognosis { get; set; }

        public int RetryTime { get; set; }

        public bool UseInterfaceServer { get; set; }

        #endregion Public Properties
    }
}