using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Dispatch;

/// <summary>
/// Validator provides methods to validate unit dispatch data.
/// </summary>
public interface IUnitDispatchValidator
{
    /// <summary>
    /// Validates dispatching unit(s).
    /// </summary>
    /// <param name="args">Unit dispatch arguments.</param>
    /// <returns>If the validation passes, collection of units will be returned.</returns>
    public Task<IList<UnitModel>> DispatchValidateAsync(UnitDispatchArgs args);
}