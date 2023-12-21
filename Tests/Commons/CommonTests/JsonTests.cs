using Commons.Enums;
using Commons.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CommonTests
{
    public class JsonTests
    {
        #region Public Methods

        [Test]
        public void BadFormedJsonStrings()
        {
            const string badFormedContent = "{\"zugnummer\":\"203\",\"decoder\":null,\"simulationszeit\":null,\"betriebsstelle\":\"XDE\",\"signaltyp\":null,\"start_gleis\":null,\"ziel_gleis\":null,\"modus\":\"prognose\"}";
            Assert.NotNull(JsonConvert.DeserializeObject<RealTimeMessage>(badFormedContent));
        }

        [Test]
        public void MessageTypes()
        {
            const string contentIstZeit = "{\"zugnummer\":\"203\",\"decoder\":null,\"simulationszeit\":\"1970-01-01 01:00:00\",\"betriebsstelle\":\"XWF_F\",\"signaltyp\":\"ESig\",\"start_gleis\":\"1\",\"ziel_gleis\":\"2\",\"modus\":\"istzeit\"}";
            Assert.True(JsonConvert.DeserializeObject<RealTimeMessage>(contentIstZeit).Modus == MessageType.IstZeit);

            const string contentPrognose = "{\"zugnummer\":\"8303\",\"decoder\":null,\"simulationszeit\":\"14:35\",\"betriebsstelle\":\"XDR_N\",\"signaltyp\":null,\"start_gleis\":null,\"ziel_gleis\":null,\"modus\":\"prognose\"}";
            Assert.True(JsonConvert.DeserializeObject<RealTimeMessage>(contentPrognose).Modus == MessageType.Prognose);
        }

        #endregion Public Methods
    }
}