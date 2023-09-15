using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Client.Services.Units.Batches;

/// <summary>
/// API service provides methods to retrieve/handle unit batch data.
/// </summary>
public interface IUnitBatchApiService
{
    /// <summary>
    /// Retrieves batch contents.
    /// </summary>
    /// <param name="whatType">What type.</param>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <param name="unitKeyIds">Collection of unit key identifiers.</param>
    /// <param name="serialKeyIds">Collection of serial key identifiers.</param>
    /// <returns>Collection of unit batch contents.</returns>
    Task<IList<UnitBatchContentsDto>> GetBatchContentsAsync(WhatType whatType, int? batchKeyId = null,
        IList<int>? unitKeyIds = null, IList<int>? serialKeyIds = null);
}