using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TDOC.Common.Converters
{
    public class JsonRowsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(string);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType is JsonToken.String)
            {
                try
                {
                    var jValue = new JValue(reader.Value);
                    return JsonConvert.DeserializeObject<IList<IList<object>>>((string)jValue);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while converting JSON string into type IList<IList<object>>: {ex.Message}");
                }
            }
            else
            {
                throw new Exception("Error while converting JSON string into type IList<IList<object>>: Object is not string type!");
            }
        }

        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jToken = JToken.FromObject(value);
            jToken.WriteTo(writer);
        }
    }
}