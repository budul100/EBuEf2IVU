namespace EBuEf2IVU.Shareds.Commons.Models
{
    public class CrewingElement
    {
        #region Public Properties

        public string BetriebsstelleNach { get; set; }

        public string BetriebsstelleVon { get; set; }

        public string DienstKurzname { get; set; }

        public string PersonalNachname { get; set; }

        public string PersonalNummer { get; set; }

        public string Zugnummer { get; set; }

        public string ZugnummerVorgaenger { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            var result = $"{Zugnummer} ({BetriebsstelleVon} - {BetriebsstelleNach}): " +
                $"{DienstKurzname ?? string.Empty} / {PersonalNachname ?? string.Empty}";

            return result;
        }

        #endregion Public Methods
    }
}