using Commons.Extensions;
using NUnit.Framework;
using System;

namespace CommonTests
{
    internal class SettingsTests
    {
        #region Public Methods

        [Test]
        public void UserEnvironmentVariables()
        {
            const string host = "test";
            Environment.SetEnvironmentVariable(SettingsExtensions.EnvironmentIVUHost, host);

            Assert.True(default(Commons.Settings.RealtimeSender).GetHost() == host);

            const int port = 9999;
            Environment.SetEnvironmentVariable(SettingsExtensions.EnvironmentIVUPort, port.ToString());

            Assert.True(default(Commons.Settings.RealtimeSender).GetPort() == port);

            const bool isHttps = true;
            Environment.SetEnvironmentVariable(SettingsExtensions.EnvironmentIVUIsHttps, isHttps.ToString());

            Assert.True(default(Commons.Settings.RealtimeSender).GetIsHttps() == isHttps);
        }

        #endregion Public Methods
    }
}