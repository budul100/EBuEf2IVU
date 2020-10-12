using Newtonsoft.Json;

namespace Common.Models
{
    public class TrainPathMessage
    {
        #region Public Properties

        [JsonProperty("trainId")]
        public string TrainId { get; set; }

        [JsonProperty("trainPathState")]
        public string TrainPathState { get; set; }

        [JsonProperty("transportationCompany")]
        public string TransportationCompany { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return $@"ZugId {TrainId} | TransportationCompany: {TransportationCompany} |TrainPathState:{TrainPathState}";
        }

        #endregion Public Methods
    }
}