using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Cancel;

/// <inheritdoc cref="IUnitCancelValidator" />
public class UnitCancelValidator : ValidatorBase<UnitModel>, IUnitCancelValidator
{
    private readonly IUnitRepository _unitRepository;
    private readonly IUnitValidator _unitValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitCancelValidator" /> class.
    /// </summary>
    /// <param name="unitRepository">Repository provides methods to retrieve/handle units.</param>
    /// <param name="unitValidator">Validator provides methods to validate units.</param>
    public UnitCancelValidator(IUnitRepository unitRepository, IUnitValidator unitValidator) : base(unitRepository)
    {
        _unitRepository = unitRepository;
        _unitValidator = unitValidator;
    }

    /// <inheritdoc />
    public async Task<IList<UnitModel>> CancelValidateAsync(UnitCancelArgs args)
    {
        ObjectNullValidate(args);
        ObjectNullValidate(args.UnitKeyIds);

        if (!args.UnitKeyIds.Any() || args.UnitKeyIds.Any(u => u == 0))
            throw ArgumentNotValidException();

        return await _unitValidator.KeyIdsValidateAsync(args.UnitKeyIds);
    }
}