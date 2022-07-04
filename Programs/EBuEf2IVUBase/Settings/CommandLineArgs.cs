using CommandLine;

namespace EBuEf2IVUBase.Settings
{
    public class CommandLineArgs
    {
        #region Public Properties

        [Option(
            shortName: 's',
            longName: "settings-path",
            Required = false,
            HelpText = "File path to the settings file.")]
        public string SettingsPath { get; set; }

        #endregion Public Properties
    }
}