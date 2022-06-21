using Common.Enums;
using System;

namespace Common.Models
{
    public class EBuEfSession
    {
        #region Public Properties

        public int FahrplanId { get; set; }

        public DateTime IVUDatum { get; set; }

        public string SessionKey { get; set; }

        public DateTime SessionStart { get; set; }

        public SessionStatusType Status { get; set; }

        public TimeSpan Verschiebung { get; set; }

        public DayOfWeek Wochentag { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return @$"EBuEf-Session | Start: {SessionStart:HH\:mm\:ss} | IVU-Datum: {IVUDatum:yyyy-MM-dd} | Wochentag: {Wochentag} | " +
                @$"Verschiebung: {(Verschiebung >= TimeSpan.Zero ? string.Empty : "-")}{Verschiebung:d\.hh\:mm\:ss} | " +
                $"Key: {SessionKey} | Fahrplan-Id: {FahrplanId} | Status: {Status}";
        }

        #endregion Public Methods
    }
}