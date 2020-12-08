using System;

namespace Common.Models
{
    public class EBuEfSession
    {
        #region Public Properties

        public DateTime IVUDatum { get; set; }

        public string SessionKey { get; set; }

        public DateTime SessionStart { get; set; }

        public TimeSpan Verschiebung { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return @$"EBuEf-Session | Key: {SessionKey} | Start: {SessionStart:hh\:mm\:ss} | IVU-Datum: {IVUDatum:yyyy-MM-dd} | " +
                @$"Verschiebung: {(Verschiebung >= TimeSpan.Zero ? string.Empty : "-")}{Verschiebung:d\.hh\:mm\:ss}";
        }

        #endregion Public Methods
    }
}