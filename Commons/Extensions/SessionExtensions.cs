using Common.Enums;
using Common.Models;
using System;

namespace Common.Extensions
{
    public static class SessionExtensions
    {
        #region Public Methods

        public static StateType? GetSessionStatusType(this string sessionStatus)
        {
            if (sessionStatus == StateType.InPreparation.ToString("D"))
            {
                return StateType.InPreparation;
            }
            else if (sessionStatus == StateType.IsRunning.ToString("D"))
            {
                return StateType.IsRunning;
            }
            else if (sessionStatus == StateType.IsEnded.ToString("D"))
            {
                return StateType.IsEnded;
            }
            else if (sessionStatus == StateType.IsPaused.ToString("D"))
            {
                return StateType.IsPaused;
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