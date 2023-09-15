namespace TDOC.Common.Client.Http
{
    /// <summary>
    /// Defines a generalized deserialization method.
    /// </summary>
    /// <typeparam name="TResult">The type of the result of the deserialization.</typeparam>
    public interface IStringDeserializer<out TResult>
    {
        /// <summary>
        /// Deserializes the specified content into an instance of the <typeparamref name="TResult"/> type.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>An instance of the <typeparamref name="TResult"/> type.</returns>
        public TResult Deserialize(string content);
    }
}