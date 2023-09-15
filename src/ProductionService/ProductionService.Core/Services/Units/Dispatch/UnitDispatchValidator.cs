using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Dispatch;

/// <inheritdoc cref="IUnitDispatchValidator" />
public class UnitDispatchValidator : ValidatorBase<UnitModel>, IUnitDispatchValidator
{
    private readonly IUnitRepository _unitRepository;
    private readonly IUnitValidator _unitValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitDispatchValidator" /> class.
    /// </summary>
    /// <param name="unitRepository">Repository provides methods to retrieve/handle units.</param>
    /// <param name="unitValidator">Validator provides methods to validate units.</param>
    public UnitDispatchValidator(IUnitRepository unitRepository, IUnitValidator unitValidator) : base(unitRepository)
    {
        _unitRepository = unitRepository;
        _unitValidator = unitValidator;
    }

    /// <inheritdoc />
    public async Task<IList<UnitModel>> DispatchValidateAsync(UnitDispatchArgs args)
    {
        ObjectNullValidate(args);
        try
        {
            await FindOtherEntityByKeyIdValidateAsync<CustomerModel>(args.CustomerKeyId);
        }
        catch (InputArgumentException exception) when (exception.Code is GenericErrorCodes.ArgumentsNotValid)
        {
            throw DomainException(DomainDispatchErrorCodes.CustomerOrReturnToStockNotSelected);
        }

        if (!args.UnitKeyIds.Any() && !args.SerialKeyIds.Any())
            throw ArgumentEmptyException();

        await FindOtherEntityByKeyIdValidateAsync<LocationModel>(args.LocationKeyId);
        await _unitValidator.KeyIdsValidateAsync(args.UnitKeyIds);
        IList<UnitModel> units = await _unitRepository.GetWithProductAndItemByKeyIdsAsync(args.UnitKeyIds);
        foreach (UnitModel unit in units)
        {
            if (unit.Expire.HasValue && unit.Expire.Value <= DateTime.Now)
                throw DomainException(DomainDispatchErrorCodes.UnitExpired, (unit.KeyId, MessageType.Title));

            if (unit.Status != (int)UnitStatus.Stock)
                throw DomainException(DomainDispatchErrorCodes.UnitStatusNotOnStock,
                    (unit.KeyId, MessageType.Title),
                    ((UnitStatus)unit.Status, MessageType.Description));
        }
        IList<UnitModel> serialUnits = await _unitRepository.GetWithProductAndItemBySerialKeyIdsAsync(args.SerialKeyIds);
        foreach (UnitModel unit in serialUnits)
        {
            if (unit.Expire.HasValue && unit.Expire.Value <= DateTime.Now)
                throw DomainException(DomainDispatchErrorCodes.SerialNumberUnitExpired,
                    (unit.Seri.SerialNo, MessageType.Title),
                    (unit.KeyId, MessageType.Description));

            if (unit.Status != (int)UnitStatus.Stock)
                throw DomainException(DomainDispatchErrorCodes.SerialNumberUnitStatusNotOnStock,
                    (unit.Seri.SerialNo, MessageType.Title),
                    (unit.KeyId, MessageType.Description),
                    ((UnitStatus)unit.Status, MessageType.Description));
        }
        return units.Concat(serialUnits).ToList();
    }
}