using System.Text.RegularExpressions;

namespace StateHandler.Extensions
{
    internal static class PatternExtensions
    {
        #region Public Fields

        public const string StatusRegexGroupName = "status";

        #endregion Public Fields

        #region Private Fields

        private const string StatusRegexGroupWildcard = "$";

        #endregion Private Fields

        #region Public Methods

        public static Regex GetSessionStartRegex(this string startPattern)
        {
            return new Regex(startPattern);
        }

        public static Regex GetSessionStatusRegex(this string statusPattern)
        {
            // The new value cannot be created with \d!

            var correctedPattern = statusPattern.Replace(
                oldValue: StatusRegexGroupWildcard,
                newValue: $"(?<{StatusRegexGroupName}>[0-9])");

            var result = new Regex(correctedPattern);

            return result;
        }

        #endregion Public Methods
    }
}