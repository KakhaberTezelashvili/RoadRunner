namespace TDOC.Common.Client.Http;

/// <inheritdoc />
public class TypedHttpClientFactory : ITypedHttpClientFactory
{
    private readonly IHttpClientFactory _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedHttpClientFactory" /> class.
    /// </summary>
    /// <param name="factory">HttpClient factory.</param>
    public TypedHttpClientFactory(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Authentication token.
    /// </summary>
    public string AuthToken { get; set; }

    /// <inheritdoc />
    public ITypedHttpClient CreateClient(string name) =>
         new TypedHttpClient(_factory.CreateClient(name), this);
}