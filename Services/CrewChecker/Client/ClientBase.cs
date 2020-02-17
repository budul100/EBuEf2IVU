using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace CrewChecker.Client
{
    internal abstract class ClientBase<TChannel>
        : IDisposable
    {
        #region Private Fields

        private const string HttpAddress = "http";
        private const string HttpsAddress = "https";

        private readonly ChannelFactory<TChannel> factory;

        #endregion Private Fields

        #region Public Constructors

        public ClientBase
            (string host, int port, string path, string userName, string password,
            bool isHttps = true, bool ignoreCertificateErrors = true)
        {
            if (ignoreCertificateErrors)
            {
                IgnoreCertificateErrors();
            }

            var binding = isHttps
                ? GetHttpsBinding()
                : GetHttpBinding();
            var endpoint = GetEndpoint(
                host: host,
                port: port,
                path: path,
                ishttps: isHttps);

            factory = GetFactory(
                binding: binding,
                endpoint: endpoint,
                userName: userName,
                password: password);

            Client = factory.CreateChannel();
        }

        #endregion Public Constructors

        #region Protected Properties

        protected TChannel Client { get; private set; }

        #endregion Protected Properties

        #region Public Methods

        public void Dispose()
        {
            factory?.Close();
        }

        #endregion Public Methods

        #region Private Methods

        private static ChannelFactory<TChannel> GetFactory
            (Binding binding, EndpointAddress endpoint, string userName, string password)
        {
            var result = new ChannelFactory<TChannel>(binding, endpoint);
            result.Credentials.UserName.UserName = userName;
            result.Credentials.UserName.Password = password;

            return result;
        }

        private static void IgnoreCertificateErrors()
        {
            ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) => { return true; };
        }

        private EndpointAddress GetEndpoint(string host, int port, string path, bool ishttps)
        {
            var url = (ishttps ? HttpsAddress : HttpAddress) + $"://{host}:{port}/{path}";

            return new EndpointAddress(new Uri(url));
        }

        private Binding GetHttpBinding()
        {
            var result = new BasicHttpBinding();

            result.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            result.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            result.MaxBufferSize = int.MaxValue;
            result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
            result.MaxReceivedMessageSize = int.MaxValue;
            result.AllowCookies = true;

            return result;
        }

        private Binding GetHttpsBinding()
        {
            var result = new BasicHttpsBinding();

            result.Security.Mode = BasicHttpsSecurityMode.Transport;
            result.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

            result.MaxBufferSize = int.MaxValue;
            result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
            result.MaxReceivedMessageSize = int.MaxValue;
            result.AllowCookies = true;

            return result;
        }

        #endregion Private Methods
    }
}