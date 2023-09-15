using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Cancel;

/// <summary>
/// Validator provides methods to validate unit cancel data.
/// </summary>
public interface IUnitCancelValidator
{
    /// <summary>
    /// Validates cancelling unit.
    /// </summary>
    /// <param name="args">Unit cancel arguments.</param>
    /// <returns>If the validation passes, collection of unit(s) will be returned.</returns>
    Task<IList<UnitModel>> CancelValidateAsync(UnitCancelArgs args);
}