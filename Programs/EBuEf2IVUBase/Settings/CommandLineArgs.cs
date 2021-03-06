using CommandLine;
using System;

namespace EBuEf2IVUBase.Settings
{
    public class CommandLineArgs
    {
        #region Public Properties

        [Option(shortName: 'l', longName: "log-path", Required = false, HelpText = "File path to the log file.")]
        public string LogPath { get; set; }

        [Option(shortName: 'c', longName: "console-application", Required = false, HelpText = "Run service as console aplication.")]
        public bool RunStamosndAlone { get; set; }

        [Option(shortName: 'd', longName: "session-date", Required = false, HelpText = "Date of the IVU session.")]
        public DateTime? SessionDateIVU { get; set; }

        [Option(shortName: 's', longName: "settings-path", Required = false, HelpText = "File path to the settings file.")]
        public string SettingsPath { get; set; }

        #endregion Public Properties
    }
}