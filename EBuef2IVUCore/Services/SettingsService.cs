using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace EBuEf2IVUCore.Services
{
    public class SettingsService<T>
        where T : class, new()
    {
        #region Private Fields

        private readonly string settingsPath;

        private readonly XmlWriterSettings writerSettings;

        #endregion Private Fields

        #region Public Constructors

        public SettingsService(string settingsPath)
        {
            this.settingsPath = settingsPath;
            writerSettings = GetWriterSettings();

            Settings = GetFromFile();
        }

        #endregion Public Constructors

        #region Public Properties

        public T Settings { get; }

        #endregion Public Properties

        #region Public Methods

        public void Save()
        {
            SaveToFile();
        }

        #endregion Public Methods

        #region Private Methods

        private static XmlWriterSettings GetWriterSettings()
        {
            return new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };
        }

        private T GetFromFile()
        {
            var result = new T();

            if (File.Exists(settingsPath))
            {
                var serializer = new XmlSerializer(typeof(T));

                using (var reader = XmlReader.Create(settingsPath))
                {
                    result = serializer.Deserialize(reader) as T;
                }
            }

            return result;
        }

        private void SaveToFile()
        {
            if (!Directory.Exists(Path.GetDirectoryName(settingsPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
            }

            var serializer = new XmlSerializer(typeof(T));
            using (var writer = XmlWriter.Create(
                outputFileName: settingsPath,
                settings: writerSettings))
            {
                serializer.Serialize(
                    xmlWriter: writer,
                    o: Settings);
            }
        }

        #endregion Private Methods
    }
}