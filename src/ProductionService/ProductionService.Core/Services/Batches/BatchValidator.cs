using ProductionService.Core.Models.Batches;
using ProductionService.Core.Services.Serials;
using ProductionService.Core.Services.Units;
using ProductionService.Shared.Dtos.Processes;

namespace ProductionService.Core.Services.Batches;

/// <inheritdoc cref="IBatchValidator" />
public class BatchValidator : ValidatorBase<BatchModel>, IBatchValidator
{
    private readonly IBatchRepository _batchRepository;
    private readonly IUnitService _unitService;
    private readonly ISerialService _serialService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BatchValidator" /> class.
    /// </summary>
    /// <param name="batchRepository">Repository provides methods to retrieve/handle batches.</param>
    /// <param name="unitService">Service provides methods to retrieve/handle units.</param>
    /// <param name="serialService">Service provides methods to retrieve/handle serial numbers.</param>
    public BatchValidator(IBatchRepository batchRepository, IUnitService unitService, ISerialService serialService) : base(batchRepository)
    {
        _batchRepository = batchRepository;
        _unitService = unitService;
        _serialService = serialService;
    }

    /// <inheritdoc />
    public async Task LinkUnitsToBatchValidateAsync(int batchKeyId, MachineType machineType, int userKeyId, BatchCreateArgs args)
    {
        ObjectNullValidate(args);

        // TODO: for POC we allowing to create empty batch.
        if (batchKeyId <= 0 || userKeyId <= 0 /*|| args.UnitKeyIds == null || !args.UnitKeyIds.Any()*/)
            throw ArgumentNotValidException();

        if (machineType != MachineType.Sterilizer && machineType != MachineType.Washer)
            throw ArgumentNotValidException();

        await AssignUnitsToSterilizeOrWashBatchValidateAsync(machineType, args.UnitKeyIds);
    }

    /// <inheritdoc />
    public async Task AssignUnitsToSterilizeOrWashBatchValidateAsync(MachineType machineType, IEnumerable<int> unitKeyIds, bool isSerialNumber = false)
    {
        if (machineType == MachineType.Sterilizer)
        {
            IList<UnitModel> units = await _unitService.GetByKeyIdsAsync(unitKeyIds);
            var unitIds = units.Where(u => u.Batch is > 0).Select(u => u.KeyId).ToList();
            if (!unitIds.Any())
                return;

            IList<BatchProcessData> batches = await _batchRepository.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(unitIds, BatchType.PrimarySteri);
            if (!batches.Any())
                return;

            BatchProcessData batch = batches.First();
            if (!isSerialNumber)
                ThrowException(batch, DomainBatchErrorCodes.UnitAlreadyRegisteredForSterilizeBatch);
            else
            {
                var unit = units.FirstOrDefault(u => u.KeyId == batch.UnitKeyId);
                SerialModel serialModel = await _serialService.GetByKeyIdAsync(unit.SeriKeyId.Value);
                throw DomainException(DomainBatchErrorCodes.SerialNumberUnitAlreadyRegisteredForSterilizeBatch,
                    (serialModel.SerialNo, MessageType.Title),
                    (batch.UnitKeyId, MessageType.Description),
                    (batch.BatchKeyId, MessageType.Description));
            }
        }
        else
        {
            IList<BatchProcessData> batches = await _batchRepository.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(unitKeyIds, BatchType.PostWash);
            BatchProcessData batch = batches.FirstOrDefault(b => b.ProcessStatus != ProcessStatus.Done);
            if (batch == null || batch.UnitKeyId == null || batch.BatchKeyId == null)
                return;

            if (!isSerialNumber)
                ThrowException(batch, DomainBatchErrorCodes.UnitAlreadyRegisteredForWashBatch);
            else
            {
                UnitModel unit = (await _unitService.GetByKeyIdsAsync(new List<int> { batch.UnitKeyId.Value })).FirstOrDefault();
                SerialModel serialModel = await _serialService.GetByKeyIdAsync(unit.SeriKeyId.Value);
                throw DomainException(DomainBatchErrorCodes.SerialNumberUnitAlreadyRegisteredForWashBatch,
                    (serialModel.SerialNo, MessageType.Title),
                    (batch.UnitKeyId, MessageType.Description),
                    (batch.BatchKeyId, MessageType.Description));
            }
        }

        void ThrowException(BatchProcessData batch, DomainBatchErrorCodes errorCode)
        {
            throw DomainException(errorCode,
                (batch.UnitKeyId, MessageType.Title),
                (batch.BatchKeyId, MessageType.Description));
        }
    }

    /// <inheritdoc />
    public void RemoveBatchesValidate(int unitKeyId)
    {
        if (unitKeyId <= 0)
            throw ArgumentNotValidException();
    }
}