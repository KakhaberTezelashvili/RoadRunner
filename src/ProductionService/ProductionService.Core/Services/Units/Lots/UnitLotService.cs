using ProductionService.Core.Repositories.Interfaces.Units;
using ProductionService.Core.Services.Units.Lots;
using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Lots;

/// <inheritdoc cref="IUnitLotService" />
public class UnitLotService : IUnitLotService
{
    private readonly IUnitLotValidator _unitLotValidator;
    private readonly IUnitLotRepository _unitLotRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitLotService" /> class.
    /// </summary>
    /// <param name="unitLotValidator">Validator provides methods to validate unit lot data.</param>
    /// <param name="unitLotRepository">Repository provides methods to retrieve/handle unit lot data.</param>
    public UnitLotService(IUnitLotValidator unitLotValidator, IUnitLotRepository unitLotRepository)
    {
        _unitLotValidator = unitLotValidator;
        _unitLotRepository = unitLotRepository;
    }

    /// <inheritdoc />
    public async Task UpdateLotsAsync(UnitLotsArgs args)
    {
        await _unitLotValidator.UpdateLotsValidateAsync(args);
        IList<UnitLotInfoModel> unitLots = await _unitLotRepository.FindByUnitKeyIdAsync(args.UnitKeyId);
        await _unitLotRepository.RemoveRangeAsync(unitLots);
        DateTime date = DateTime.Now;

        IEnumerable<UnitLotInfoModel> newUnitLots = args.LotInformationList
            .Select(ule => new UnitLotInfoModel
            {
                UnitUnit = args.UnitKeyId,
                CreatedKeyId = args.UserKeyId,
                LocaKeyId = args.LocationKeyId,
                Created = date,
                LotInKeyId = ule.KeyId,
                UlstPosition = ule.Position
            });
        await _unitLotRepository.AddRangeAsync(newUnitLots);
    }
}