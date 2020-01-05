using System;

namespace Common.Models
{
    public class EBuEfSession
    {
        #region Public Properties

        public DateTime IVUDate { get; set; }

        public DateTime SessionStart { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString()
        {
            return $"EBuEf-Session - Start: {SessionStart} | IVU-Datum: {IVUDate:yyyy-MM-dd}";
        }

        #endregion Public Methods
    }
}