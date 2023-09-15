using ProductionService.Core.Repositories.Interfaces.Units;
using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Lots;

/// <inheritdoc cref="IUnitLotValidator" />
public class UnitLotValidator : ValidatorBase<UnitLotInfoModel>, IUnitLotValidator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitLotValidator" /> class.
    /// </summary>
    /// <param name="unitLotRepository">Repository provides methods to retrieve/handle unit lot data.</param>
    public UnitLotValidator(IUnitLotRepository unitLotRepository) : base(unitLotRepository)
    {
    }

    /// <inheritdoc />
    public async Task UpdateLotsValidateAsync(UnitLotsArgs args)
    {
        ObjectNullValidate(args);
        
        await FindOtherEntityByKeyIdValidateAsync<UnitModel>(args.UnitKeyId);
        await FindOtherEntityByKeyIdValidateAsync<LocationModel>(args.LocationKeyId);
        await FindOtherEntityByKeyIdValidateAsync<UserModel>(args.UserKeyId);

        if (args.LotInformationList == null || !args.LotInformationList.Any())
            throw ArgumentNotValidException();
    }
}
