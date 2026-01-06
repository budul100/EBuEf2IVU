using System;
using EBuEf2IVU.Shareds.Commons.Extensions;
using EBuEf2IVU.Shareds.Commons.Settings;

namespace EBuEf2IVU.Shareds.CommonsTests
{
    public class SettingsTests
    {
        #region Public Methods

        [Fact]
        public void UserEnvironmentVariables()
        {
            const string endpoint = "testpoint";
            Environment.SetEnvironmentVariable(RealtimeSender.EnvironmentIVUIFEndpoint, endpoint);

            Assert.Equal(endpoint, default(RealtimeSender).GetIVUIFServerEndpoint());

            const string host = "testhost";
            Environment.SetEnvironmentVariable(ConnectorIVUBase.EnvironmentHost, host);

            Assert.Equal(host, default(RealtimeSender).GetIVUAppServerHost());

            const int port = 9999;
            Environment.SetEnvironmentVariable(ConnectorIVUBase.EnvironmentPort, port.ToString());

            Assert.Equal(port, default(RealtimeSender).GetIVUAppServerPort());

            const bool isHttps = true;
            Environment.SetEnvironmentVariable(ConnectorIVUBase.EnvironmentIsHttps, isHttps.ToString());

            Assert.Equal(isHttps, default(RealtimeSender).IsIVUAppServerHttps());
        }

        #endregion Public Methods
    }
}