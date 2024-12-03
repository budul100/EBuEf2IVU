using System;
using Commons.Extensions;
using Commons.Settings;
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
            Environment.SetEnvironmentVariable(RealtimeSender.EnvironmentIVUIFEndpoint, endpoint);

            Assert.That(default(Commons.Settings.RealtimeSender).GetIVUIFServerEndpoint(), Is.EqualTo(endpoint));

            const string host = "testhost";
            Environment.SetEnvironmentVariable(RealtimeSender.EnvironmentHost, host);

            Assert.That(default(Commons.Settings.RealtimeSender).GetIVUAppServerHost(), Is.EqualTo(host));

            const int port = 9999;
            Environment.SetEnvironmentVariable(RealtimeSender.EnvironmentPort, port.ToString());

            Assert.That(default(Commons.Settings.RealtimeSender).GetIVUAppServerPort(), Is.EqualTo(port));

            const bool isHttps = true;
            Environment.SetEnvironmentVariable(RealtimeSender.EnvironmentIsHttps, isHttps.ToString());

            Assert.That(default(Commons.Settings.RealtimeSender).IsIVUAppServerHttps(), Is.EqualTo(isHttps));
        }

        #endregion Public Methods
    }
}