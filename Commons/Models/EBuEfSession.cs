using Common.Enums;
using System;

namespace Common.Models
{
    public class EBuEfSession
    {
        #region Public Properties

        public int FahrplanId { get; set; }

        public DateTime IVUDatum { get; set; }

        public string Name { get; set; }

        public string SessionKey { get; set; }

        public DateTime SessionStart { get; set; }

        public StateType Status { get; set; }

        public TimeSpan Verschiebung { get; set; }

        public DayOfWeek Wochentag { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return $"EBuEf-Session '{Name}' | Status: {Status} | Key: {SessionKey} | Fahrplan-Id: {FahrplanId} | " +
                @$"Start: {SessionStart:HH\:mm\:ss} | IVU-Datum: {IVUDatum:yyyy-MM-dd} | Wochentag: {Wochentag} | " +
                @$"Verschiebung: {(Verschiebung >= TimeSpan.Zero ? string.Empty : "-")}{Verschiebung:d\.hh\:mm\:ss}";
        }

        #endregion Public Methods
    }
}