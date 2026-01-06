using CommandLine;
using EBuEf2IVU.Shareds.Commons.Enums;

namespace EBuEf2IVU.Shareds.Commons.Settings
{
    public class CommandLineArgs
    {
        #region Public Fields

        public const string EnvironmentServiceType = "EBUEF2IVU_SERVICE";
        public const string EnvironmentSettingsPath = "EBUEF2IVU_SETTINGS";

        #endregion Public Fields

        #region Public Properties

        [Value(
            index: 0,
            Required = false,
            HelpText = "The service to be started.")]
        public ServiceType ServiceType { get; set; }

        [Option(
            shortName: 's',
            longName: "settings-path",
            Required = false,
            HelpText = "File path to the settings file.")]
        public string SettingsPath { get; set; }

        #endregion Public Properties
    }
}