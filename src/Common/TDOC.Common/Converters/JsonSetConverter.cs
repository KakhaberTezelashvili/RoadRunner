using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TDOC.Common.Utilities;

namespace TDOC.Common.Converters
{
    public class JsonSetConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(int);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType is JsonToken.Integer)
            {
                try
                {
                    var jValue = new JValue(reader.Value);
                    var hash = new HashSet<T> { };
                    foreach (T item in Enum.GetValues(typeof(T)))
                    {
                        if (BitUtilities.IsBitSet((byte)Convert.ToInt32(item), (int)jValue))
                        {
                            hash.Add(item);
                        }
                    }
                    return hash;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while converting Json string into type ISet<T>: {ex.Message}");
                }
            }
            else
            {
                throw new Exception("Error while converting Json string into type ISet<T>: Object is not integer type!");
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