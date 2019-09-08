using CommandLine;

namespace EBuEf2IVUCore.Internals
{
    internal class CommandLineOptions
    {
        #region Public Properties

        [Option(shortName: 'a', longName: "stand-alone", Required = false, HelpText = "Run service as stand-alone aplication.")]
        public bool RunStandAlone { get; set; }

        [Option(shortName: 'f', longName: "settings-path", Required = false, HelpText = "File path to the settings file.")]
        public string SettingsPath { get; set; }

        #endregion Public Properties
    }
}