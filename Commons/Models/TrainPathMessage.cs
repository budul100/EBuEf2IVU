using System;
using Commons.Converters;
using Newtonsoft.Json;

namespace Commons.Models
{
    public class TrainPathMessage
    {
        #region Public Properties

        [JsonProperty("abfahrt")]
        public DateTime? Abfahrt { get; set; }

        [JsonProperty("abfahrt_exakt")]
        public DateTime? AbfahrtExakt { get; set; }

        [JsonProperty("abfahrt_ist")]
        public DateTime? AbfahrtIst { get; set; }

        [JsonProperty("abfahrt_plan")]
        public DateTime? AbfahrtPlan { get; set; }

        [JsonProperty("abfahrt_prognose")]
        public DateTime? AbfahrtPrognose { get; set; }

        [JsonProperty("abfahrt_soll")]
        public DateTime? AbfahrtSoll { get; set; }

        [JsonProperty("ankunft")]
        public DateTime? Ankunft { get; set; }

        [JsonProperty("ankunft_exakt")]
        public DateTime? AnkunftExakt { get; set; }

        [JsonProperty("ankunft_ist")]
        public DateTime? AnkunftIst { get; set; }

        [JsonProperty("ankunft_plan")]
        public DateTime? AnkunftPlan { get; set; }

        [JsonProperty("ankunft_prognose")]
        public DateTime? AnkunftPrognose { get; set; }

        [JsonProperty("ankunft_soll")]
        public DateTime? AnkunftSoll { get; set; }

        [JsonProperty("bemerkungen")]
        public string Bemerkungen { get; set; }

        [JsonProperty("betriebsstelle")]
        public string Betriebsstelle { get; set; }

        [JsonProperty("bremssystem")]
        public string Bremssystem { get; set; }

        [JsonProperty("fzm_id")]
        public string FzmId { get; set; }

        [JsonProperty("gleis")]
        public int? Gleis { get; set; }

        [JsonProperty("gleis_ist")]
        public int? GleisIst { get; set; }

        [JsonProperty("gleis_plan")]
        public int? GleisPlan { get; set; }

        [JsonProperty("gleis_soll")]
        public int? GleisSoll { get; set; }

        [JsonProperty("ist_durchfahrt"),
            JsonConverter(typeof(IntToBooleanConverter))]
        public bool IstDurchfahrt { get; set; }

        [JsonProperty("mbr")]
        public string Mbr { get; set; }

        [JsonProperty("triebfahrzeug")]
        public string Triebfahrzeug { get; set; }

        [JsonProperty("triebfahrzeug_ist")]
        public string TriebfahrzeugIst { get; set; }

        [JsonProperty("uebergang_nach_zug_id")]
        public int? UebergangNachZugId { get; set; }

        [JsonProperty("uebergang_von_zug_id")]
        public int? UebergangVonZugId { get; set; }

        [JsonProperty("verkehrstage")]
        public string Verkehrstage { get; set; }

        [JsonProperty("verkehrstage_bin")]
        public string VerkehrstageBin { get; set; }

        [JsonProperty("vmax")]
        public int Vmax { get; set; }

        [JsonProperty("vmax_ist")]
        public int VmaxIst { get; set; }

        [JsonProperty("wendezug")]
        public string Wendezug { get; set; }

        [JsonProperty("zug")]
        public string Zug { get; set; }

        [JsonProperty("zuggattung")]
        public string Zuggattung { get; set; }

        [JsonProperty("zuggattung_id")]
        public string ZuggattungId { get; set; }

        [JsonProperty("zug_id")]
        public int ZugId { get; set; }

        [JsonProperty("zugnummer")]
        public int Zugnummer { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return $"ZugId {ZugId} | Betriebsstelle: {Betriebsstelle}";
        }

        #endregion Public Methods
    }
}