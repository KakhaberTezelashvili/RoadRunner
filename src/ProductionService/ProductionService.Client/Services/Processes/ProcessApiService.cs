using ProductionService.Shared.Dtos.Processes;

namespace ProductionService.Client.Services.Process;

/// <inheritdoc cref="IProcessApiService" />
public class ProcessApiService : IProcessApiService
{
    private const string urlPathBase = "processes";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public ProcessApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.ProductionClient);
    }

    /// <inheritdoc />
    public async Task<int> CreateBatchAsync(BatchCreateArgs args) =>
        await _httpClient.PostWithResultAsync<BatchCreateArgs, int>($"{urlPathBase}/batch", args);

    /// <inheritdoc />
    public Task ApproveBatchAsync(int batchKeyId, BatchApproveArgs args) =>
        _httpClient.PutAsync($"{urlPathBase}/{batchKeyId}/approve", args);

    /// <inheritdoc />
    public Task DisapproveBatchAsync(int batchKeyId, BatchDisapproveArgs args) =>
        _httpClient.PutAsync($"{urlPathBase}/{batchKeyId}/disapprove", args);

    /// <inheritdoc />
    public async Task<BatchDetailsDto> GetBatchDetailsAsync(int batchKeyId) =>
        await _httpClient.GetAsync<BatchDetailsDto>($"{urlPathBase}/{batchKeyId}");
}