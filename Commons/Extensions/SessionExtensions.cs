using Common.Enums;
using Common.Models;
using System;

namespace Common.Extensions
{
    public static class SessionExtensions
    {
        #region Public Methods

        public static SessionStatusType? GetSessionStatusType(this string sessionStatus)
        {
            if (sessionStatus == SessionStatusType.InPreparation.ToString("D"))
            {
                return SessionStatusType.InPreparation;
            }
            else if (sessionStatus == SessionStatusType.IsRunning.ToString("D"))
            {
                return SessionStatusType.IsRunning;
            }
            else if (sessionStatus == SessionStatusType.IsEnded.ToString("D"))
            {
                return SessionStatusType.IsEnded;
            }
            else if (sessionStatus == SessionStatusType.IsPaused.ToString("D"))
            {
                return SessionStatusType.IsPaused;
            }
            else
            {
                return default;
            }
        }

        public static DateTime GetSimTime(this EBuEfSession ebuefSession)
        {
            var sessionShift = -ebuefSession.Verschiebung;

            var result = sessionShift == default
                ? DateTime.Now
                : DateTime.Now.Add(sessionShift);

            return result;
        }

        #endregion Public Methods
    }
}