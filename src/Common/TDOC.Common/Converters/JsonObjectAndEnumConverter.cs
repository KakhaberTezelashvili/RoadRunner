using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TDOC.Common.Converters
{
    /// <summary>
    /// Serialize/Deserialize Value property of <see cref="Models.Errors.ValidationCodeDetails"/> class.
    /// </summary>
    public class JsonObjectAndEnumConverter : JsonConverter
    {
        private const string enumPostfix = ".Enum";

        public override bool CanConvert(Type objectType) => objectType.IsSubclassOf(typeof(object));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                // If the value is not string, convert with default properties.
                if (reader.TokenType != JsonToken.String)
                    return serializer.Deserialize(reader);
                else
                {
                    var jValue = new JValue(reader.Value);
                    string value = jValue.ToString();

                    // If the value is not Enum, convert with default properties, else convert to Enum.
                    if (!value.EndsWith(enumPostfix))
                        return serializer.Deserialize(reader);
                    else
                        return StringToEnum(value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while converting Json into type object: {ex.Message}");
            }
        }

        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is not Enum)
            {
                // When the value is not Enum, deserialize according to default properties.
                var jToken = JToken.FromObject(value, serializer);
                jToken.WriteTo(writer);
            }
            else
            {
                // When the value is Enum, convert into unique string representation.
                var jToken = JToken.FromObject($"{value.GetType().AssemblyQualifiedName}.{(int)value}{enumPostfix}", serializer);
                jToken.WriteTo(writer);
            }
        }

        /// <summary>
        /// Converts string representation of Enum to Enum.
        /// </summary>
        /// <param name="enumString">String representation of enum value with EnumPostfix before serialization.</param>
        /// <returns>Enum.</returns>
        private Enum StringToEnum(string enumString)
        {
            int enumPostFixIndex = enumString.LastIndexOf(enumPostfix);
            enumString = enumString.Substring(0, enumPostFixIndex);
            int lastDotIndex = enumString.LastIndexOf('.');
            string enumTypeString = enumString[..lastDotIndex];
            var enumType = Type.GetType(enumTypeString);
            int enumValue = int.Parse(enumString[(lastDotIndex + 1)..]);
            return (Enum)Enum.ToObject(enumType, enumValue);
        }
    }
}