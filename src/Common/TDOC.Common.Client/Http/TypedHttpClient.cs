using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TDOC.Common.Client.Http;

/// <inheritdoc />
public class TypedHttpClient : ITypedHttpClient
{
    private readonly JsonSerializerOptions _jsonSerializationOptions;
    private readonly HttpClient _httpClient;
    private readonly ITypedHttpClientFactory _httpClientFactory;

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedHttpClient" /> class.
    /// </summary>
    public TypedHttpClient()
    {
        _jsonSerializationOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedHttpClient" /> class.
    /// </summary>
    /// <param name="name">HttpClient name.</param>
    /// <param name="factory">HttpClient factory.</param>
    public TypedHttpClient(HttpClient httpClient, ITypedHttpClientFactory httpClientFactory) : this()
    {
        _httpClient = httpClient;
        _httpClientFactory = httpClientFactory;
    }

    #endregion Constructors

    #region Static methods

    /// <summary>
    /// Serializes the specified value into Json and wraps it in a <see cref="StringContent"/> instance with
    /// default encoding (UFT8) and media type (application/json).
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="newtonsoft">Use Newtonsoft for JSON conversion.</param>
    /// <returns>A <see cref="StringContent"/> instance containing the serialized value.</returns>
    public HttpContent ToJsonContent<TValue>(TValue value, bool newtonsoft = false)
    {
        string json = newtonsoft ?
            Newtonsoft.Json.JsonConvert.SerializeObject(value) :
            JsonSerializer.Serialize(value, _jsonSerializationOptions);

        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    /// <summary>
    /// Creates a <see cref="StringContent"/> instance with the specified value, encoding and media type.
    /// </summary>
    /// <param name="value">The value of the content.</param>
    /// <param name="encoding">The encoding of the content.</param>
    /// <param name="mediaType">The media type of the content</param>
    /// <returns>A <see cref="StringContent"/> instance.</returns>
    public static HttpContent ToStringContent(string value, Encoding encoding, string mediaType) => 
        new StringContent(value, encoding, mediaType);

    #endregion Static methods

    /// <inheritdoc />
    public string ClientBaseAddress() => _httpClient.BaseAddress.ToString();

    #region Delete requests

    /// <inheritdoc />
    public async Task DeleteAsync(string requestUri)
    {
        UseBearerAuthentication();
        HttpResponseMessage response = await _httpClient.DeleteAsync(requestUri);
        response.EnsureSuccessStatusCode();
    }

    #endregion Delete requests

    #region Get requests

    /// <summary>
    /// Protected. Sends a GET request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="deserializer">An optional custom deserializer used for deserializing the response
    /// content into the specified type, <typeparamref name="TResult"/>.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result is the response content deserialized into the specified type, <typeparamref name="TResult"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="requestUri"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    protected async Task<TResult> DoGetAsync<TResult>(string requestUri,
        IStringDeserializer<TResult> deserializer = null)
    {
        UseBearerAuthentication();
        HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        // Deserialize into Json.
        if (deserializer == null)
            return await response.Content.ReadAsAsync<TResult>();
        // Deserialize into custom format.
        string content = await response.Content.ReadAsStringAsync();
        return deserializer.Deserialize(content);
    }

    /// <inheritdoc />
    public async Task<TResult> GetAsync<TResult>(string requestUri) => await DoGetAsync<TResult>(requestUri);

    /// <inheritdoc />
    public async Task<TResult> GetAsync<TResult>(string requestUri, IStringDeserializer<TResult> deserializer)
    {
        if (deserializer == null)
            throw new ArgumentNullException(nameof(deserializer));
        return await DoGetAsync(requestUri, deserializer);
    }

    public async Task<string> GetStringAsync(string requestUri)
    {
        UseBearerAuthentication();
        HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<byte[]> GetBytesAsync(string requestUri)
    {
        UseBearerAuthentication();
        HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    #endregion Get requests

    #region Patch requests

    /// <summary>
    /// Protected. Sends a PATCH request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value sent with the request.</typeparam>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">The value sent with the request.</param>
    /// <param name="serializer">An optional custom serializer used for serializing the value into a string.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="requestUri"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    protected async Task DoPatchAsync<TValue>(string requestUri, TValue value,
        IValueSerializer<TValue> serializer = null)
    {
        using HttpContent content = (serializer == null) ?
            ToJsonContent(value) : ToStringContent(serializer.Serialize(value), serializer.Encoding, serializer.MediaType);
        UseBearerAuthentication();
        HttpResponseMessage response = await _httpClient.PatchAsync(requestUri, content);
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task PatchAsync<TValue>(string requestUri, TValue value) => await DoPatchAsync(requestUri, value);

    /// <inheritdoc />
    public async Task PatchAsync<TValue>(string requestUri, TValue value, IValueSerializer<TValue> serializer)
    {
        if (serializer == null)
            throw new ArgumentNullException(nameof(serializer));
        await DoPatchAsync(requestUri, value, serializer);
    }

    #endregion Patch requests

    #region Post requests without result

    /// <summary>
    /// Protected. Sends a POST request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">Value to be posted.</param>
    /// <param name="serializer">An optional custom serializer used for serializing the value into a string.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="requestUri"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    protected async Task DoPostWithoutResultAsync<TValue>(string requestUri, TValue value,
        IValueSerializer<TValue> serializer = null)
    {
        using HttpContent content = (serializer == null) ?
            ToJsonContent(value) : ToStringContent(serializer.Serialize(value), serializer.Encoding, serializer.MediaType);
        UseBearerAuthentication();
        HttpResponseMessage response = await _httpClient.PostAsync(requestUri, content);
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task PostWithoutResultAsync<TValue>(string requestUri, TValue value) => await DoPostWithoutResultAsync(requestUri, value);

    /// <inheritdoc />
    public async Task PostWithoutResultAsync<TValue>(string requestUri, TValue value,
        IValueSerializer<TValue> serializer)
    {
        if (serializer == null)
            throw new ArgumentNullException(nameof(serializer));
        await DoPostWithoutResultAsync(requestUri, value, serializer);
    }

    #endregion Post requests without result

    #region Post requests with result

    /// <summary>
    /// Protected. Sends a POST request to the specified Uri as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to post.</typeparam>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">Value to be posted.</param>
    /// <param name="serializer">An optional custom deserializer used for deserializing the response
    /// content into the specified type, <typeparamref name="TResult"/></param>
    /// <param name="deserializer">An optional custom deserializer used for deserializing the response
    /// <param name="newtonsoft">Use Newtonsoft for JSON conversion.</param>
    /// content into the specified type, <typeparamref name="TResult"/>.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result is the response content deserialized into the specified type, <typeparamref name="TResult"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="requestUri"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    protected async Task<TResult> DoPostWithResultAsync<TValue, TResult>(string requestUri, TValue value,
        IValueSerializer<TValue> serializer = null, IStringDeserializer<TResult> deserializer = null, bool newtonsoft = false)
    {
        using HttpContent content = (serializer == null) ?
            ToJsonContent(value, newtonsoft) : ToStringContent(serializer.Serialize(value), serializer.Encoding, serializer.MediaType);
        UseBearerAuthentication();
        HttpResponseMessage response = await _httpClient.PostAsync(requestUri, content);
        response.EnsureSuccessStatusCode();

        // Deserialize into Json.
        if (deserializer == null)
            return await response.Content.ReadAsAsync<TResult>();
        // Deserialize into custom format.
        string responseContent = await response.Content.ReadAsStringAsync();
        return deserializer.Deserialize(responseContent);
    }

    /// <inheritdoc />
    public async Task<TResult> PostWithResultAsync<TValue, TResult>(string requestUri, TValue value, bool newtonsoft = false) =>
        await DoPostWithResultAsync<TValue, TResult>(requestUri, value, newtonsoft: newtonsoft);

    /// <inheritdoc />
    public async Task<TResult> PostWithResultAsync<TValue, TResult>(string requestUri, TValue value,
        IValueSerializer<TValue> serializer)
    {
        if (serializer == null)
            throw new ArgumentNullException(nameof(serializer));
        return await DoPostWithResultAsync<TValue, TResult>(requestUri, value, serializer);
    }

    /// <inheritdoc />
    public async Task<TResult> PostWithResultAsync<TValue, TResult>(string requestUri, TValue value,
        IStringDeserializer<TResult> deserializer)
    {
        if (deserializer == null)
            throw new ArgumentNullException(nameof(deserializer));
        return await DoPostWithResultAsync(requestUri, value, null, deserializer);
    }

    /// <inheritdoc />
    public async Task<TResult> PostWithResultAsync<TValue, TResult>(string requestUri, TValue value,
        IValueSerializer<TValue> serializer, IStringDeserializer<TResult> deserializer)
    {
        if (serializer == null)
            throw new ArgumentNullException(nameof(serializer));
        if (deserializer == null)
            throw new ArgumentNullException(nameof(deserializer));
        return await DoPostWithResultAsync(requestUri, value, serializer, deserializer);
    }

    #endregion Post requests with result

    #region Put requests

    /// <summary>
    /// Sends a PUT request to the specified Uri as an asynchronous operation. This method is protected.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to be put.</typeparam>
    /// <param name="requestUri">The Uri the request is sent to.</param>
    /// <param name="value">The value to be put.</param>
    /// <param name="serializer">An optional custom serializer used for serializing the value into a string.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="requestUri"/> is <c>null</c>.</exception>
    /// <exception cref="HttpRequestException">The request failed.</exception>
    protected async Task DoPutAsync<TValue>(string requestUri, TValue value,
        IValueSerializer<TValue> serializer = null)
    {
        using HttpContent content = (serializer == null) ?
            ToJsonContent(value) : ToStringContent(serializer.Serialize(value), serializer.Encoding, serializer.MediaType);
        UseBearerAuthentication();
        HttpResponseMessage response = await _httpClient.PutAsync(requestUri, content);
        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task PutAsync<TValue>(string requestUri, TValue value) => await DoPutAsync(requestUri, value);

    /// <inheritdoc />
    public async Task PutAsync<TValue>(string requestUri, TValue value, IValueSerializer<TValue> serializer)
    {
        if (serializer == null)
            throw new ArgumentNullException(nameof(serializer));
        await DoPutAsync(requestUri, value, serializer);
    }

    #endregion Put requests

    /// <summary>
    /// Configures the client to use basic authentication.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    private void UseBasicAuthentication(string username, string password)
    {
        string authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", authToken);
    }

    private void UseBearerAuthentication()
    {
        if (!string.IsNullOrEmpty(_httpClientFactory.AuthToken))
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _httpClientFactory.AuthToken);
    }
}