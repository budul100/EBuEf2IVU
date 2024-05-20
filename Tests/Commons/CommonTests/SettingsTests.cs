using System;
using Commons.Extensions;
using NUnit.Framework;

namespace CommonTests
{
    internal class SettingsTests
    {
        #region Public Methods

        [Test]
        public void UserEnvironmentVariables()
        {
            const string endpoint = "testpoint";
            Environment.SetEnvironmentVariable(SettingsExtensions.EnvironmentIVUIFEndpoint, endpoint);

            Assert.That(default(Commons.Settings.RealtimeSender).GetIVUIFServerEndpoint(), Is.EqualTo(endpoint));

            const string host = "testhost";
            Environment.SetEnvironmentVariable(SettingsExtensions.EnvironmentIVUAppHost, host);

            Assert.That(default(Commons.Settings.RealtimeSender).GetIVUAppServerHost(), Is.EqualTo(host));

            const int port = 9999;
            Environment.SetEnvironmentVariable(SettingsExtensions.EnvironmentIVUAppPort, port.ToString());

            Assert.That(default(Commons.Settings.RealtimeSender).GetIVUAppServerPort(), Is.EqualTo(port));

            const bool isHttps = true;
            Environment.SetEnvironmentVariable(SettingsExtensions.EnvironmentIVUAppSecure, isHttps.ToString());

            Assert.That(default(Commons.Settings.RealtimeSender).GetIVUAppServerSecure(), Is.EqualTo(isHttps));
        }

        #endregion Public Methods
    }
}