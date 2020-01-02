using CommandLine;
using System;

namespace EBuEf2IVUCore.Models
{
    internal class CommandLineOptions
    {
        #region Public Properties

        [Option(shortName: 'p', longName: "performance-test", Required = false, HelpText = "Run a number of performance tests.")]
        public int RunPerformanceTestRounds { get; set; }

        [Option(shortName: 'a', longName: "stand-alone", Required = false, HelpText = "Run service as stand-alone aplication.")]
        public bool RunStandAlone { get; set; }

        [Option(shortName: 'd', longName: "session-date", Required = false, HelpText = "Date of the IVU session.")]
        public DateTime? SessionDateIVU { get; set; }

        [Option(shortName: 'f', longName: "settings-path", Required = false, HelpText = "File path to the settings file.")]
        public string SettingsPath { get; set; }

        #endregion Public Properties
    }
}