namespace TDOC.Common.Client.Http;

/// <summary>
/// Defines methods and properties for performing asynchronous HTTP requests with strongly typed serialization and deserialization of request and response content.
/// By default, request/response content is serialized and deserialized into Json, but custom serializers and deserializers can be provided to
/// support other formats such as Xml and proprietary formats;
/// refer to the <see cref="IValueSerializer{TValue}"/> and <see cref="IStringDeserializer{TResult}"/> interfaces.
/// </summary>
public interface ITypedHttpClient
{
    /// <summary>
    /// Get http client base address.
    /// </summary>
    /// <returns></returns>
    string ClientBaseAddress();

    /// <summary>
    /// Sends a DELETE request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="requestUri"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    Task DeleteAsync(string requestUri);

    /// <summary>
    /// Sends a GET request to the specified Uri as an asynchronous operation.
    /// and returns the response as the specified type.
    /// </summary>
    /// <typeparam name="TResult">The type of the response content.</typeparam>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result is the response content deserialized into the specified type, <typeparamref name="TResult"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="requestUri"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    Task<TResult> GetAsync<TResult>(string requestUri);

    /// <summary>
    /// Sends a GET request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TResult">The type of the response content.</typeparam>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="deserializer">Custom deserializer used for deserializing the response
    /// content into the specified type, <typeparamref name="TResult"/>.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result is the response content deserialized into the specified type, <typeparamref name="TResult"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="deserializer"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="requestUri"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    Task<TResult> GetAsync<TResult>(string requestUri, IStringDeserializer<TResult> deserializer);

    /// <summary>
    /// Sends a GET request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result is the response content deserialized into string.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="requestUri"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    Task<string> GetStringAsync(string requestUri);

    /// <summary>
    /// Sends a GET request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result is the response content deserialized into byte[].</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="requestUri"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    Task<byte[]> GetBytesAsync(string requestUri);

    /// <summary>
    /// Sends a PATCH request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to be used for the patch.</typeparam>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">Value sent with the request.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="requestUri"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    Task PatchAsync<TValue>(string requestUri, TValue value);

    /// <summary>
    /// Sends a PATCH request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to be used for the patch.</typeparam>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">Value sent with the request.</param>
    /// <param name="serializer">Custom serializer used for serializing the value into a string.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Either <paramref name="requestUri"/> or <paramref name="serializer"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    Task PatchAsync<TValue>(string requestUri, TValue value, IValueSerializer<TValue> serializer);

    /// <summary>
    /// Sends a POST request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to be posted.</typeparam>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">Value to be posted.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="requestUri"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    Task PostWithoutResultAsync<TValue>(string requestUri, TValue value);

    /// <summary>
    /// Sends a POST request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to be posted.</typeparam>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">Value to be posted.</param>
    /// <param name="serializer">Custom serializer used for serializing the value into a string.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="serializer"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="requestUri"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    Task PostWithoutResultAsync<TValue>(string requestUri, TValue value, IValueSerializer<TValue> serializer);

    /// <summary>
    /// Sends a POST request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to be posted.</typeparam>
    /// <typeparam name="TResult">The type of the response content.</typeparam>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">Value to be posted.</param>
    /// <param name="newtonsoft">Use Newtonsoft for JSON conversion.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result is the response content deserialized into the specified type, <typeparamref name="TResult"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="requestUri"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    Task<TResult> PostWithResultAsync<TValue, TResult>(string requestUri, TValue value, bool newtonsoft = false);

    /// <summary>
    /// Sends a POST request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to be posted.</typeparam>
    /// <typeparam name="TResult">The type of the response content.</typeparam>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">Value to be posted.</param>
    /// <param name="serializer">Custom deserializer used for deserializing the response
    /// content into the specified type, <typeparamref name="TResult"/>.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result is the response content deserialized into the specified type, <typeparamref name="TResult"/>.</returns>
    /// <exception cref="ArgumentNullException">Either <paramref name="requestUri"/> or <paramref name="serializer"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    Task<TResult> PostWithResultAsync<TValue, TResult>(string requestUri, TValue value, IValueSerializer<TValue> serializer);

    /// <summary>
    /// Sends a POST request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to be posted.</typeparam>
    /// <typeparam name="TResult">The type of the response content.</typeparam>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">Value to be posted.</param>
    /// <param name="deserializer">Custom deserializer used for deserializing the response
    /// content into the specified type, <typeparamref name="TResult"/>.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result is the response content deserialized into the specified type, <typeparamref name="TResult"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="deserializer"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="requestUri"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    Task<TResult> PostWithResultAsync<TValue, TResult>(string requestUri, TValue value,
        IStringDeserializer<TResult> deserializer);

    /// <summary>
    /// Sends a POST request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to be posted.</typeparam>
    /// <typeparam name="TResult">The type of the response content.</typeparam>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">Value to be posted.</param>
    /// <param name="serializer">Custom serializer used for serializing the value into a string.</param>
    /// <param name="deserializer">Custom deserializer used for deserializing the response
    /// content into the specified type, <typeparamref name="TResult"/>.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result is the response content deserialized into the specified type, <typeparamref name="TResult"/>.</returns>
    /// <exception cref="ArgumentNullException">Either <paramref name="serializer"/> or <paramref name="deserializer"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="requestUri"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    Task<TResult> PostWithResultAsync<TValue, TResult>(string requestUri, TValue value, IValueSerializer<TValue> serializer,
        IStringDeserializer<TResult> deserializer);

    /// <summary>
    /// Sends a PUT request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to be put.</typeparam>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">The value to be put.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="requestUri"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    Task PutAsync<TValue>(string requestUri, TValue value);

    /// <summary>
    /// Sends a PUT request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to be put.</typeparam>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">The value to be put.</param>
    /// <param name="serializer">Custom serializer used for serializing the value into a string.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="serializer"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="requestUri"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    Task PutAsync<TValue>(string requestUri, TValue value, IValueSerializer<TValue> serializer);
}