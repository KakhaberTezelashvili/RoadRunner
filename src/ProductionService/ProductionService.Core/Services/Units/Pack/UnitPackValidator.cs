using Microsoft.Extensions.DependencyInjection;
using ProductionService.Core.Models.Batches;
using ProductionService.Core.Models.Units;
using ProductionService.Core.Repositories.Interfaces.Units;
using ProductionService.Core.Services.Batches;
using ProductionService.Core.Services.Serials;
using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Pack;

/// <inheritdoc cref="IUnitPackValidator" />
public class UnitPackValidator : ValidatorBase<UnitModel>, IUnitPackValidator
{
    private readonly IUnitPackRepository _unitPackRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISerialService _serialService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitPackValidator" /> class.
    /// </summary>
    /// <param name="unitPackRepository">Repository provides methods to retrieve/handle unit pack data.</param>
    /// <param name="serviceProvider">Service provider to access dependencies when there is circular dependency.</param>
    /// <param name="serialService">Service provides methods to retrieve/handle serial numbers.</param>
    public UnitPackValidator(
        IUnitPackRepository unitPackRepository,
        IServiceProvider serviceProvider,
        ISerialService serialService) : base(unitPackRepository)
    {
        _unitPackRepository = unitPackRepository;
        _serviceProvider = serviceProvider;
        _serialService = serialService;
    }

    /// <inheritdoc />
    public async Task<UnitDataToPack> PackValidateAsync(UnitPackArgs args)
    {
        ObjectNullValidate(args);

        // At least one of keys needed to pack unit.
        bool result = args.UnitKeyId != 0 || args.ProductSerialKeyId != 0 || args.ProductKeyId != 0;

        // The rest values check.
        bool isValid = result &&
                      args.LocationKeyId != 0 &&
                      args.FactoryKeyId != 0 &&
                      args.PositionLocationKeyId != 0 &&
                      args.Amount > 0;
        if (!isValid)
            throw ArgumentNotValidException();

        UnitDataToPack packData = await _unitPackRepository.GetDataToPackAsync(args);
        if (packData == null)
            throw ArgumentNotFoundException();

        if (packData.NextUnit != null)
            throw DomainException(DomainPackErrorCodes.UnitAlreadyPackedFromUnit, (packData.UnitKeyId, MessageType.Title));

        SerialModel serial = null;
        if (args.ProductSerialKeyId != 0)
        {
            serial = await _serialService.GetByKeyIdAsync(packData.ProductSerialKeyId.Value);
            if (serial == null)
                throw ArgumentNotFoundException();

            if (serial.Status >= (int)TDocConstants.ObjectStatus.Dead)
                throw ArgumentNotValidException();
        }

        // If unit status is different than returned.
        if (packData.UnitStatus.HasValue && packData.UnitStatus != (int)UnitStatus.Returned)
        {
            if (args.ProductSerialKeyId != 0)
            {
                throw DomainException(DomainPackErrorCodes.SerialNumberUnitStatusNotReturned,
                    (serial.SerialNo, MessageType.Title),
                    (packData.UnitKeyId, MessageType.Description),
                    ((UnitStatus?)packData.UnitStatus, MessageType.Description));
            }
            else
            {
                throw DomainException(DomainPackErrorCodes.UnitStatusNotReturned,
                    (packData.UnitKeyId, MessageType.Title),
                    ((UnitStatus?)packData.UnitStatus, MessageType.Description));
            }
        }
        if (args.ProductSerialKeyId != 0 && !packData.UnitKeyId.HasValue)
            return packData;
        // In case we are packing from product we don't need to check wash batch approved.
        if (args.ProductKeyId == 0)
            await CheckWashingOkForPackAsync(packData.UnitKeyId.Value, args.ProductSerialKeyId);

        return packData;
    }

    /// <summary>
    /// Check if a unit's wash batch is ok.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <param name="productSerialKeyId">Product serial key identifier.</param>
    private async Task CheckWashingOkForPackAsync(int unitKeyId, int productSerialKeyId)
    {
        IServiceScope scope = _serviceProvider.CreateScope();
        IBatchService batchService = scope.ServiceProvider.GetRequiredService<IBatchService>();
        IEnumerable<BatchProcessData> batches = await batchService.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(
            new List<int>() { unitKeyId }, BatchType.PostWash);
        BatchProcessData batchProcessData = batches?.FirstOrDefault();

        // One or more washer processes are required. ProcBatch will be 0 if the unit is not in
        // ANY batch.
        if (batchProcessData == null || batchProcessData.ProcessBatch == null || batchProcessData.ProcessBatch == 0)
        {
            if (productSerialKeyId != 0)
                await ThrowErrorWhenPackMissingWashBatchByProductSerialAsync(productSerialKeyId, unitKeyId);
            else
                ThrowErrorWhenPackMissingWashBatch(unitKeyId);
        }

        // No matter if washing is required on the product or not, it is possible that a unit
        // has been registered to a washer batch anyway. If it has, then this washer batch must
        // have been:
        // - Approved OR
        // - Not require approval and then not have any error attached. So we must ALWAYS find
        // the latest Washer Batch a unit has been, or is, in and make sure that batch was done
        // and OK.
        bool unitIsInBlockingBatch =
           !(batchProcessData.ProcessStatus == ProcessStatus.Done
               && ((batchProcessData.ProcessApprovedUserKeyId > 0)
                   || (batchProcessData.ProgramApproval == ApprovalType.No
                       && batchProcessData.ProcessError == 0)));

        if (unitIsInBlockingBatch)
        {
            SerialModel serial = null;
            if (productSerialKeyId != 0)
                serial = await _serialService.GetByKeyIdAsync(productSerialKeyId);
            throw DomainException(productSerialKeyId != 0
                    ? DomainPackErrorCodes.SerialNumberUnitWashBatchNotApproved
                    : DomainPackErrorCodes.UnitWashBatchNotApproved,
                (productSerialKeyId != 0 ? serial.SerialNo : unitKeyId, MessageType.Title),
                (batchProcessData.ProcessBatch, MessageType.Description));
        }
    }

    private void ThrowErrorWhenPackMissingWashBatch(int unitKeyId)
    {
        var validationCodeDetails = new List<ValidationCodeDetails>
        {
            new() { Value = unitKeyId, MessageType = MessageType.Title }
        };
        throw new DomainException(DomainPackErrorCodes.UnitMissingWashBatch, validationCodeDetails);
    }

    private async Task ThrowErrorWhenPackMissingWashBatchByProductSerialAsync(int productSerialKeyId, int unitKeyId)
    {
        SerialModel serial = await _serialService.GetByKeyIdAsync(productSerialKeyId);
        throw DomainException(DomainPackErrorCodes.SerialNumberUnitMissingWashBatch,
            (serial.SerialNo, MessageType.Title),
            (unitKeyId, MessageType.Description));
    }
}