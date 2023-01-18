using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TrainPathSender.Extensions
{
    internal static class SerializerExtensions
    {
        #region Public Methods

        public static string Serialize<T>(this IEnumerable<T> contents)
            where T : class
        {
            var result = new StringBuilder();

            if (contents?.Any() ?? false)
            {
                var relevants = contents
                    .Select(c => c.Serialize())
                    .Where(t => !string.IsNullOrWhiteSpace(t)).ToArray();

                foreach (var relevant in relevants)
                {
                    result.AppendLine(relevant);
                }
            }

            return result.ToString();
        }

        #endregion Public Methods

        #region Private Methods

        private static string Serialize<T>(this T content)
            where T : class
        {
            var result = default(string);

            if (content != default)
            {
                var serializer = new XmlSerializer(typeof(T));

                using var stringWriter = new StringWriter();

                using var xmlWriter = new XmlTextWriter(stringWriter);

                serializer.Serialize(
                    xmlWriter: xmlWriter,
                    o: content);

                result = stringWriter.ToString();
            }

            return result;
        }

        #endregion Private Methods
    }
}