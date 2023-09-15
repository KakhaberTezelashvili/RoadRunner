namespace TDOC.Common.Client.Http;

/// <summary>
/// Factory used for construction <see cref="ITypedHttpClient"/> interfaces.
/// </summary>
public interface ITypedHttpClientFactory
{
    /// <summary>
    /// Creates http client depending on configuration name.
    /// </summary>
    /// <param name="name">Http client configuration name.</param>
    /// <returns><see cref="ITypedHttpClient"/>.</returns>
    ITypedHttpClient CreateClient(string name);

    /// <summary>
    /// Authentication token.
    /// </summary>
    string AuthToken { get; set; }
}