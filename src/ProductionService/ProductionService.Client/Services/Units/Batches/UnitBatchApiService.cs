using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Client.Services.Units.Batches;

/// <inheritdoc />
public class UnitBatchApiService : IUnitBatchApiService
{
    private const string urlPathBase = "units";
    private readonly ITypedHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitBatchApiService" /> class.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    public UnitBatchApiService(ITypedHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientConfigurationName.ProductionClient);
    }

    /// <inheritdoc />
    public async Task<IList<UnitBatchContentsDto>> GetBatchContentsAsync(WhatType whatType, int? batchKeyId = null, 
        IList<int>? unitKeyIds = null, IList<int>? serialKeyIds = null)
    {
        string queryParams = $"?whatType={(int)whatType}";

        if (batchKeyId.HasValue)
            queryParams += $"&batchId={batchKeyId}";

        if (unitKeyIds != null)
            queryParams += $"&unitIds={string.Join(",", unitKeyIds)}";

        if (serialKeyIds != null)
            queryParams += $"&serialIds={string.Join(",", serialKeyIds)}";

        return await _httpClient.GetAsync<IList<UnitBatchContentsDto>>($"{urlPathBase}/batch-contents{queryParams}");
    }
}