using Newtonsoft.Json;
using System;

namespace Commons.Converters
{
    public class IntToBooleanConverter
        : JsonConverter
    {
        #region Public Methods

        public override bool CanConvert(Type objectType)
        {
            var result = objectType == typeof(string) || objectType == typeof(bool);

            return result;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value;

            return !string.IsNullOrWhiteSpace(value?.ToString())
                && value.Equals("1")
                ? true
                : (object)false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}