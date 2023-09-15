using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Errors;

/// <summary>
/// Validator provides methods to validate unit error data.
/// </summary>
public interface IUnitErrorValidator : IValidatorBase<UnitModel>
{
    /// <summary>
    /// Validates updating the unit with the specified error.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier</param>
    /// <param name="args">Unit error arguments.</param>
    /// <returns>If the validation passes, unit will be returned.</returns>
    Task<UnitModel> UpdateErrorValidateAsync(int unitKeyId, UnitErrorArgs args);
}