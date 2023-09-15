using ProductionService.Core.Services.Batches;
using ProductionService.Shared.Dtos.Units;
using ProductionService.Shared.Enumerations.Errors;

namespace ProductionService.Core.Services.Units.Cancel;

/// <inheritdoc cref="IUnitCancelService" />
public class UnitCancelService : IUnitCancelService
{
    private readonly IUnitRepository _unitRepository;
    private readonly IUnitCancelValidator _unitCancelValidator;
    private readonly IUnitValidator _unitValidator;
    private readonly IBatchService _batchService;
    private readonly ISerialRepository _serialRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitCancelService" /> class.
    /// </summary>
    /// <param name="unitRepository">Repository provides methods to retrieve/handle units.</param>
    /// <param name="unitCancelValidator">Validator provides methods to validate unit cancel data.</param>
    /// <param name="unitValidator">Validator provides methods to validate units.</param>
    /// <param name="batchService">Service provides methods to retrieve/handle batches.</param>
    /// <param name="serialRepository">Repository provides methods to retrieve/handle serial numbers.</param>
    public UnitCancelService(
        IUnitRepository unitRepository,
        IUnitCancelValidator unitCancelValidator,
        IUnitValidator unitValidator,
        IBatchService batchService,
        ISerialRepository serialRepository)
    {
        _unitRepository = unitRepository;
        _unitCancelValidator = unitCancelValidator;
        _unitValidator = unitValidator;
        _batchService = batchService;
        _serialRepository = serialRepository;
    }

    /// <inheritdoc />
    public async Task CancelAsync(UnitCancelArgs args)
    {
        // Cancelling unit
        IList<UnitModel> units = await _unitCancelValidator.CancelValidateAsync(args);
        foreach (UnitModel unit in units)
        {
            await UpdateSerialData(unit.SeriKeyId, unit.PrevUnit, unit.UsageCounter - 1);

            var oldStatus = (UnitStatus)unit.Status;

            unit.Status = (int)UnitStatus.ErrorReg;
            unit.Error = (int)FixErrorCodes.ErrorReg;
            unit.StokKeyId = null;
            await _unitRepository.UpdateAsync(unit);
            await _batchService.RemoveBatchesByUnitKeyIdAndUnitStatusAsync(unit.KeyId, oldStatus);

            // Clear "NextUnit" property for previous unit
            if (unit.PrevUnit != null)
            {
                UnitModel previousUnit = (await _unitValidator.KeyIdsValidateAsync(new List<int> { (int)unit.PrevUnit })).FirstOrDefault();
                previousUnit.NextUnit = null;
                await _unitRepository.UpdateAsync(previousUnit);
            }
        }
    }

    private async Task UpdateSerialData(int? seriKeyId, int? newUnitValue, int? usageCounter)
    {
        if (seriKeyId > 0)
        {
            SerialModel serialModel = await _unitValidator.FindOtherEntityByKeyIdValidateAsync<SerialModel>((int)seriKeyId);
            serialModel.UnitUnit = newUnitValue > 0 ? newUnitValue.Value : null;
            serialModel.UsageCount = usageCounter <= 0 ? null : usageCounter;
            await _serialRepository.UpdateAsync(serialModel);
        }
    }
}