using Newtonsoft.Json;

namespace Common.Models
{
    public class TrainPathMessage
    {
        #region Public Properties

        [JsonProperty("abfahrt")]
        public string Abfahrt { get; set; }

        [JsonProperty("abfahrt_exakt")]
        public string AbfahrtExakt { get; set; }

        [JsonProperty("abfahrt_ist")]
        public object AbfahrtIst { get; set; }

        [JsonProperty("abfahrt_plan")]
        public string AbfahrtPlan { get; set; }

        [JsonProperty("abfahrt_prognose")]
        public object AbfahrtPrognose { get; set; }

        [JsonProperty("abfahrt_soll")]
        public string AbfahrtSoll { get; set; }

        [JsonProperty("ankunft")]
        public string Ankunft { get; set; }

        [JsonProperty("ankunft_exakt")]
        public string AnkunftExakt { get; set; }

        [JsonProperty("ankunft_ist")]
        public object AnkunftIst { get; set; }

        [JsonProperty("ankunft_plan")]
        public string AnkunftPlan { get; set; }

        [JsonProperty("ankunft_prognose")]
        public object AnkunftPrognose { get; set; }

        [JsonProperty("ankunft_soll")]
        public string AnkunftSoll { get; set; }

        [JsonProperty("bemerkungen")]
        public string Bemerkungen { get; set; }

        [JsonProperty("betriebsstelle")]
        public string Betriebsstelle { get; set; }

        [JsonProperty("bremssystem")]
        public string Bremssystem { get; set; }

        [JsonProperty("fahrtrichtung")]
        public string Fahrtrichtung { get; set; }

        [JsonProperty("fzm_id")]
        public string FzmId { get; set; }

        [JsonProperty("gleis")]
        public object Gleis { get; set; }

        [JsonProperty("gleis_ist")]
        public object GleisIst { get; set; }

        [JsonProperty("gleis_plan")]
        public object GleisPlan { get; set; }

        [JsonProperty("gleis_soll")]
        public object GleisSoll { get; set; }

        [JsonProperty("ins_gegengleis")]
        public string InsGegengleis { get; set; }

        [JsonProperty("ist_durchfahrt")]
        public string IstDurchfahrt { get; set; }

        [JsonProperty("ist_kurzeinfahrt")]
        public string IstKurzeinfahrt { get; set; }

        [JsonProperty("mbr")]
        public string Mbr { get; set; }

        [JsonProperty("triebfahrzeug")]
        public string Triebfahrzeug { get; set; }

        [JsonProperty("triebfahrzeug_ist")]
        public string TriebfahrzeugIst { get; set; }

        [JsonProperty("uebergang_nach_zug_id")]
        public string UebergangNachZugId { get; set; }

        [JsonProperty("uebergang_von_zug_id")]
        public object UebergangVonZugId { get; set; }

        [JsonProperty("verkehrstage")]
        public string Verkehrstage { get; set; }

        [JsonProperty("verkehrstage_bin")]
        public string VerkehrstageBin { get; set; }

        [JsonProperty("vmax")]
        public string Vmax { get; set; }

        [JsonProperty("vmax_ist")]
        public string VmaxIst { get; set; }

        [JsonProperty("wendezug")]
        public string Wendezug { get; set; }

        [JsonProperty("zug")]
        public string Zug { get; set; }

        [JsonProperty("zuggattung")]
        public string Zuggattung { get; set; }

        [JsonProperty("zuggattung_id")]
        public string ZuggattungId { get; set; }

        [JsonProperty("zug_id")]
        public string ZugId { get; set; }

        [JsonProperty("zugnummer")]
        public string Zugnummer { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return $@"ZugId {ZugId} | Betriebsstelle: {Betriebsstelle}";
        }

        #endregion Public Methods
    }
}