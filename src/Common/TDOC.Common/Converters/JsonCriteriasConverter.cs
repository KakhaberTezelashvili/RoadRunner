using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serialize.Linq.Nodes;
using TDOC.Common.Binders;

namespace TDOC.Common.Converters
{
    public class JsonCriteriasConverter : JsonConverter
    {
        private readonly KnownTypesBinder knownTypesBinder = new()
        {
            KnownTypes = new List<Type>
            {
                typeof(BinaryExpressionNode),
                typeof(ConditionalExpressionNode),
                typeof(ConstantExpressionNode),
                typeof(ExpressionNode),
                typeof(LambdaExpressionNode),
                typeof(MemberExpressionNode),
                typeof(MemberInfoNode),
                typeof(MethodCallExpressionNode),
                typeof(MethodInfoNode),
                typeof(ParameterExpressionNode),
                typeof(TypeNode),
                typeof(UnaryExpressionNode)
            }
        };

        public override bool CanConvert(Type objectType) => objectType == typeof(ExpressionNode);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType is JsonToken.StartObject)
            {
                try
                {
                    serializer.TypeNameHandling = TypeNameHandling.Objects;
                    serializer.SerializationBinder = knownTypesBinder;
                    return serializer.Deserialize(reader);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while converting JSON string into type ExpressionNode: {ex.Message}");
                }
            }
            else if (reader.TokenType is JsonToken.Null)
            {
                return null;
            }
            else
            {
                throw new Exception("Error while converting JSON string into type ExpressionNode: Object is not string type!");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.TypeNameHandling = TypeNameHandling.Objects;
            serializer.SerializationBinder = knownTypesBinder;
            var jToken = JToken.FromObject(value, serializer);
            jToken.WriteTo(writer);
        }
    }
}