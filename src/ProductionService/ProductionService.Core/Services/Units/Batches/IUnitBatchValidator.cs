using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Batches;

/// <summary>
/// Validator provides methods to validate unit batch data.
/// </summary>
public interface IUnitBatchValidator : IValidatorBase<UnitModel>
{
    /// <summary>
    /// Validates retrieving batch contents.
    /// </summary>
    /// <param name="whatType">What type.</param>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <param name="unitKeyIds">Collection of unit key identifiers.</param>
    /// <param name="serialKeyIds">Collection of serial key identifiers.</param>
    /// <returns>Collection of unit batch contents.</returns>
    Task<IList<UnitBatchContentsDto>> GetBatchContentsValidateAsync(
        WhatType whatType, int? batchKeyId, IList<int> unitKeyIds, IList<int> serialKeyIds = null);
}