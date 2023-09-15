using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Batches;

/// <summary>
/// Service provides methods to retrieve/handle unit batch data.
/// </summary>
public interface IUnitBatchService
{
    /// <summary>
    /// Retrieves batch contents.
    /// </summary>
    /// <param name="whatType">What type.</param>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <param name="unitKeyIds">Collection of unit key identifiers.</param>
    /// <param name="serialKeyIds">Collection of serial key identifiers.</param>
    /// <returns>Collection of unit batch contents.</returns>
    Task<IList<UnitBatchContentsDto>> GetBatchContentsAsync(WhatType whatType, int? batchKeyId, IList<int> unitKeyIds, IList<int> serialKeyIds = null);
}