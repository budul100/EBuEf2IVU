using Commons.Enums;
using System;

namespace Commons.Models
{
    public class EBuEfSession
    {
        #region Public Properties

        public int FahrplanId { get; set; }

        public DateTime IVUDatum { get; set; }

        public string Name { get; set; }

        public TimeSpan RealStart { get; set; }

        public string SessionKey { get; set; }

        public TimeSpan SessionStart { get; set; }

        public StateType Status { get; set; }

        public TimeSpan Verschiebung { get; set; }

        public DayOfWeek Wochentag { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            var verschiebungVorzeichen = Verschiebung >= TimeSpan.Zero ? string.Empty : "-";

            return $"EBuEf-Session '{Name}' | Status: {Status} | Key: {SessionKey} | Fahrplan-Id: {FahrplanId} | " +
                @$"IVU-Datum: {IVUDatum:yyyy-MM-dd} | Wochentag: {Wochentag} | Start (real): {RealStart:hh\:mm\:ss} | " +
                @$"Verschiebung: {verschiebungVorzeichen}{Verschiebung:d\.hh\:mm\:ss} | Start (Sim): {SessionStart:hh\:mm\:ss}";
        }

        #endregion Public Methods
    }
}