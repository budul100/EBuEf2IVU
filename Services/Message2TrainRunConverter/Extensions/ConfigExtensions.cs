using System;
using Microsoft.Extensions.Configuration;
using Commons.Models;

namespace Message2TrainRunConverter.Extensions
{
    internal static class ConfigExtensions
    {
        #region Public Methods

        public static Func<TrainPathMessage, DateTime?> GetAbfahrtGetter(this IConfiguration config)
        {
            var senderSettings = config
                .GetSection(nameof(Commons.Settings.TrainPathSender))
                .Get<Commons.Settings.TrainPathSender>();

            var result = senderSettings.PreferPrognosis
                ? (Func<TrainPathMessage, DateTime?>)(m => m.AbfahrtPrognose ?? m.AbfahrtSoll ?? m.AbfahrtPlan)
                : (Func<TrainPathMessage, DateTime?>)(m => m.AbfahrtSoll ?? m.AbfahrtPlan);

            return result;
        }

        public static Func<TrainPathMessage, DateTime?> GetAnkunftGetter(this IConfiguration config)
        {
            var senderSettings = config
                .GetSection(nameof(Commons.Settings.TrainPathSender))
                .Get<Commons.Settings.TrainPathSender>();

            var result = senderSettings.PreferPrognosis
                ? (Func<TrainPathMessage, DateTime?>)(m => m.AnkunftPrognose ?? m.AnkunftSoll ?? m.AnkunftPlan)
                : (Func<TrainPathMessage, DateTime?>)(m => m.AnkunftSoll ?? m.AnkunftPlan);

            return result;
        }

        #endregion Public Methods
    }
}