using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Errors;

/// <inheritdoc />
public class UnitErrorService : IUnitErrorService
{
    private readonly IUnitErrorValidator _unitErrorValidator;
    private readonly IUnitRepository _unitRepository;

    // <summary>
    /// Initializes a new instance of the <see cref="UnitErrorService" /> class.
    /// </summary>
    /// <param name="unitErrorValidator">Validator provides methods to validate unit error data.</param>
    /// <param name="unitRepository">Repository provides methods to retrieve/handle units.</param>
    public UnitErrorService(IUnitErrorValidator unitErrorValidator, IUnitRepository unitRepository)
    {
        _unitErrorValidator = unitErrorValidator;
        _unitRepository = unitRepository;
    }

    /// <inheritdoc />
    public async Task UpdateErrorAsync(int unitKeyId, UnitErrorArgs args)
    {
        UnitModel unit = await _unitErrorValidator.UpdateErrorValidateAsync(unitKeyId, args);
        unit.Error = args.ErrorNumber;
        await _unitRepository.UpdateAsync(unit);
    }
}