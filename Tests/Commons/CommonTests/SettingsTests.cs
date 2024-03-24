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
            Environment.SetEnvironmentVariable(SettingsExtensions.EnvironmentIVUEndpoint, endpoint);

            Assert.That(default(Commons.Settings.RealtimeSender).GetEndpoint(), Is.EqualTo(endpoint));

            const string host = "testhost";
            Environment.SetEnvironmentVariable(SettingsExtensions.EnvironmentIVUHost, host);

            Assert.That(default(Commons.Settings.RealtimeSender).GetHost(), Is.EqualTo(host));

            const int port = 9999;
            Environment.SetEnvironmentVariable(SettingsExtensions.EnvironmentIVUPort, port.ToString());

            Assert.That(default(Commons.Settings.RealtimeSender).GetPort(), Is.EqualTo(port));

            const bool isHttps = true;
            Environment.SetEnvironmentVariable(SettingsExtensions.EnvironmentIVUIsHttps, isHttps.ToString());

            Assert.That(default(Commons.Settings.RealtimeSender).GetIsHttps(), Is.EqualTo(isHttps));
        }

        #endregion Public Methods
    }
}