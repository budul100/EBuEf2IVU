using ConverterExtensions;
using EBuEf2IVU.Shareds.Commons.Models;

namespace EBuEf2IVU.Services.DatabaseConnector.Extensions
{
    internal static class LegExtensions
    {
        #region Public Methods

        public static int? GetEBuEfGleis(this TrainLeg leg, bool istVon)
        {
            var result = istVon
                ? leg.EBuEfGleisVon.ToInt()
                : leg.EBuEfGleisNach.ToInt();

            return result;
        }

        #endregion Public Methods
    }
}