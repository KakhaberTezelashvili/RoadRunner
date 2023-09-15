using ProductionService.Core.Models.Batches;
using ProductionService.Core.Services.Units;
using ProductionService.Core.Services.Units.Locations;
using ProductionService.Shared.Dtos.Processes;

namespace ProductionService.Core.Services.Batches;

/// <inheritdoc cref="IBatchService" />
public class BatchService : IBatchService
{
    private readonly IBatchRepository _batchRepository;
    private readonly IBatchValidator _batchValidator;
    private readonly IUnitService _unitService;
    private readonly IUnitLocationService _unitLocationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BatchService" /> class.
    /// </summary>
    /// <param name="batchRepository">Repository provides methods to retrieve/handle batches.</param>
    /// <param name="batchValidator">Validator provides methods to validate batches.</param>
    /// <param name="unitService">Service provides methods to retrieve/handle units.</param>
    /// <param name="unitLocationService">Service provides methods to retrieve/handle unit locations.</param>
    public BatchService(
        IBatchRepository batchRepository,
        IBatchValidator batchValidator,
        IUnitService unitService,
        IUnitLocationService unitLocationService)
    {
        _batchRepository = batchRepository;
        _batchValidator = batchValidator;
        _unitService = unitService;
        _unitLocationService = unitLocationService;
    }

    /// <inheritdoc />
    public async Task LinkUnitsToBatchAsync(int batchKeyId, MachineType machineType, int userKeyId, BatchCreateArgs args)
    {
        await _batchValidator.LinkUnitsToBatchValidateAsync(batchKeyId, machineType, userKeyId, args);

        // Remember "batch type" based on machineType (Sterilizer/Washer).
        BatchType batchType = machineType == MachineType.Sterilizer ? BatchType.PrimarySteri : BatchType.PostWash;

        // Remember "what happened" based on machineType (Sterilizer/Washer).
        WhatType whatType = machineType == MachineType.Sterilizer ? WhatType.SteriPreBatch : WhatType.WashPreBatch;

        foreach (int unitKeyId in args.UnitKeyIds)
        {
            DateTime locationTime = DateTime.Now;
            await AddUnitToBatch(batchKeyId, batchType, unitKeyId, args.LocationKeyId, locationTime);
            await _unitLocationService.AddAsync(
                unitKeyId, userKeyId, args.LocationKeyId, args.PositionLocationKeyId, locationTime, whatType);

            UnitStatus status = UnitStatus.None;
            // TODO: case Process.Options.PreDisUnitAssignStatus of
            // pduasOpened: Status := statOPENED;
            // pduasUsed: Status := statUSED;

            if (status != UnitStatus.None)
            {
                await _unitService.UpdateUnitStatusOrErrorAsync(unitKeyId, (int)status);
            }
        }
    }

    /// <inheritdoc />
    public async Task UpdateBatchesStatusToOkAsync(int batchKeyId)
    {
        IEnumerable<BatchModel> batches = await _batchRepository.FindUnFailedBatchesByBatchKeyIdAsync(batchKeyId);

        foreach (BatchModel batch in batches)
            batch.Status = BatchUnitStatus.OK;

        await _batchRepository.UpdateRangeAsync(batches);
    }

    /// <inheritdoc />
    public async Task UpdateBatchesStatusToFailedAsync(int batchKeyId)
    {
        IEnumerable<BatchModel> batches = await _batchRepository.FindByBatchKeyIdAsync(batchKeyId);

        foreach (BatchModel batch in batches)
        {
            batch.Status = BatchUnitStatus.BatchFailed;
        }

        await _batchRepository.UpdateRangeAsync(batches);
    }

    /// <inheritdoc />
    public async Task RemoveBatchesByUnitKeyIdAndUnitStatusAsync(int unitKeyId, UnitStatus unitStatus)
    {
        _batchValidator.RemoveBatchesValidate(unitKeyId);

        BatchType batchType = unitStatus == UnitStatus.Init ? BatchType.PreWash : BatchType.PostWash;
        IEnumerable<BatchModel> batches = await _batchRepository.FindByUnitKeyIdAndBatchTypeAsync(unitKeyId, batchType);

        await _batchRepository.RemoveRangeAsync(batches);
    }

    /// <inheritdoc />
    public async Task<IList<BatchProcessData>> GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(IEnumerable<int> unitKeyIds, BatchType batchType) => 
        await _batchRepository.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(unitKeyIds, batchType);

    /// <summary>
    /// Adds unit to the batch.
    /// </summary>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <param name="batchType">Batch type.</param>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <param name="locationKeyId">Location key identifier.</param>
    /// <param name="locationTime">Location time.</param>
    /// <returns></returns>
    private async Task AddUnitToBatch(int batchKeyId, BatchType batchType, int unitKeyId, int locationKeyId, DateTime locationTime)
    {
        var batch = new BatchModel
        {
            Batch = batchKeyId,
            Unit = unitKeyId,
            Type = (int)batchType
        };
        await _batchRepository.AddAsync(batch);

        await _unitLocationService.UpdateAsync(batchKeyId, batchType, unitKeyId, locationKeyId, locationTime);
    }
}