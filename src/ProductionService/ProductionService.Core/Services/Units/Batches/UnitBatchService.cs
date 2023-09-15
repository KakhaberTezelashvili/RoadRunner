using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Batches;

/// <inheritdoc />
public class UnitBatchService : IUnitBatchService
{
    private readonly IUnitBatchValidator _unitBatchValidator;

    // <summary>
    /// Initializes a new instance of the <see cref="UnitBatchService" /> class.
    /// </summary>
    /// <param name="unitBatchValidator">Validator provides methods to validate unit batch data.</param>
    public UnitBatchService(IUnitBatchValidator unitBatchValidator)
    {
        _unitBatchValidator = unitBatchValidator;
    }

    /// <inheritdoc />
    public async Task<IList<UnitBatchContentsDto>> GetBatchContentsAsync(WhatType whatType, int? batchKeyId, IList<int> unitKeyIds, IList<int> serialKeyIds = null) =>
        await _unitBatchValidator.GetBatchContentsValidateAsync(whatType, batchKeyId, unitKeyIds, serialKeyIds);
}