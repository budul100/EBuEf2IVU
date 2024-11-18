using System;
using System.IO;
using System.Reflection;
using EBuEf2IVUBase.Settings;
using StringExtensions;

namespace EBuEf2IVUBase.Extensions
{
    public static class CommandLineArgsExtensions
    {
        #region Public Methods

        public static string GetSettingsPath(this CommandLineArgs args, string defaultSettingsPath)
        {
            var result = args.SettingsPath.IsEmpty()
                ? defaultSettingsPath
                : args.SettingsPath;

            if (!File.Exists(result))
            {
                var localResult = Path.Combine(
                    path1: Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    path2: result);

                if (File.Exists(localResult))
                {
                    result = localResult;
                }
            }

            if (!File.Exists(result))
            {
                throw new ApplicationException($"The settings file '{result}' cannot be found.");
            }

            return result;
        }

        #endregion Public Methods
    }
}