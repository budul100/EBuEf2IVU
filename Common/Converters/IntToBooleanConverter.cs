using Newtonsoft.Json;
using System;

namespace Common.Converters
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

            if (!string.IsNullOrWhiteSpace(value?.ToString())
                && value.Equals("1"))
            {
                return true;
            }

            return false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}