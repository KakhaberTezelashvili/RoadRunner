using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units;

/// <summary>
/// Validator provides methods to validate units.
/// </summary>
public interface IUnitValidator : IValidatorBase<UnitModel>
{
    /// <summary>
    /// Validates collection of unit key identifiers.
    /// </summary>
    /// <param name="unitKeyIds">Collection of unit key identifiers.</param>
    /// <returns>Collection of unit(s).</returns>
    Task<IList<UnitModel>> KeyIdsValidateAsync(IList<int> unitKeyIds);
}