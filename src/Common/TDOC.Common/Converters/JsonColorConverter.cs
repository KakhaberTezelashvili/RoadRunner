using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;

namespace TDOC.Common.Converters
{
    public class JsonColorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(int);

        public static Color DelphiColorToColor(int delphiColor) => 
            Color.FromArgb(0xFF, delphiColor & 0xFF, (delphiColor >> 8) & 0xFF, (delphiColor >> 16) & 0xFF);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType is JsonToken.Integer)
            {
                try
                {
                    var jValue = new JValue(reader.Value);
                    return DelphiColorToColor((int)jValue);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while converting Json string into type Color: {ex.Message}");
                }
            }
            else if (reader.TokenType is JsonToken.Null)
            {
                return null;
            }
            else
            {
                throw new Exception("Error while converting Json string into type Color: Object is not Delphi ARGB color!");
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