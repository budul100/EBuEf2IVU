using System;
using Microsoft.Extensions.Configuration;
using EBuEf2IVU.Shareds.Commons.Models;
using EBuEf2IVU.Shareds.Commons.Settings;

namespace EBuEf2IVU.Services.Message2TrainRunConverter.Extensions
{
    internal static class ConfigExtensions
    {
        #region Public Methods

        public static Func<TrainPathMessage, DateTime?> GetAbfahrtGetter(this IConfiguration config)
        {
            var senderSettings = config
                .GetSection(nameof(TrainPathSender))
                .Get<TrainPathSender>();

            var result = senderSettings.PreferPrognosis
                ? (m => m.AbfahrtPrognose ?? m.AbfahrtSoll ?? m.AbfahrtPlan)
                : (Func<TrainPathMessage, DateTime?>)(m => m.AbfahrtSoll ?? m.AbfahrtPlan);

            return result;
        }

        public static Func<TrainPathMessage, DateTime?> GetAnkunftGetter(this IConfiguration config)
        {
            var senderSettings = config
                .GetSection(nameof(TrainPathSender))
                .Get<TrainPathSender>();

            var result = senderSettings.PreferPrognosis
                ? (m => m.AnkunftPrognose ?? m.AnkunftSoll ?? m.AnkunftPlan)
                : (Func<TrainPathMessage, DateTime?>)(m => m.AnkunftSoll ?? m.AnkunftPlan);

            return result;
        }

        #endregion Public Methods
    }
}