using EBuEf2IVU.Shareds.Commons.Enums;
using EBuEf2IVU.Shareds.Commons.Models;
using Newtonsoft.Json;

namespace EBuEf2IVU.Shareds.CommonsTests
{
    public class JsonTests
    {
        #region Public Methods

        [Fact]
        public void BadFormedJsonStrings()
        {
            const string badFormedContent = "{\"zugnummer\":\"203\",\"decoder\":null,\"simulationszeit\":null,\"betriebsstelle\":\"XDE\",\"signaltyp\":null,\"start_gleis\":null,\"ziel_gleis\":null,\"modus\":\"prognose\"}";
            var result = JsonConvert.DeserializeObject<RealTimeMessage>(badFormedContent);
            Assert.NotNull(result);
        }

        [Fact]
        public void MessageTypes()
        {
            const string contentIstZeit = "{\"zugnummer\":\"203\",\"decoder\":null,\"simulationszeit\":\"1970-01-01 01:00:00\",\"betriebsstelle\":\"XWF_F\",\"signaltyp\":\"ESig\",\"start_gleis\":\"1\",\"ziel_gleis\":\"2\",\"modus\":\"istzeit\"}";
            Assert.Equal(
                MessageType.IstZeit,
                JsonConvert.DeserializeObject<RealTimeMessage>(contentIstZeit).Modus);

            const string contentPrognose = "{\"zugnummer\":\"8303\",\"decoder\":null,\"simulationszeit\":\"14:35\",\"betriebsstelle\":\"XDR_N\",\"signaltyp\":null,\"start_gleis\":null,\"ziel_gleis\":null,\"modus\":\"prognose\"}";
            Assert.Equal(
                MessageType.Prognose,
                JsonConvert.DeserializeObject<RealTimeMessage>(contentPrognose).Modus);
        }

        #endregion Public Methods
    }
}