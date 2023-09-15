using Newtonsoft.Json.Serialization;

namespace TDOC.Common.Binders
{
    /// <summary>
    /// There is vulnerability to use TypeNameHandling.All, TypeNameHandling.Objects.
    /// To prevent this it is needed to allow only known types for serialization and
    /// deserialization.
    /// </summary>
    public class KnownTypesBinder : ISerializationBinder
    {
        public IList<Type> KnownTypes { get; set; }

        public Type BindToType(string assemblyName, string typeName) => KnownTypes.FirstOrDefault(t => t.Name == typeName);

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }
    }
}
