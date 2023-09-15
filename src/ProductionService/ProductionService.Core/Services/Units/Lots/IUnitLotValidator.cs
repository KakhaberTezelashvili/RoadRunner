using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Lots;

/// <summary>
/// Validator provides methods to validate unit lot data.
/// </summary>
public interface IUnitLotValidator
{
    /// <summary>
    /// Validates updating unit lots.
    /// </summary>
    /// <param name="args">Unit lots arguments.</param>
    Task UpdateLotsValidateAsync(UnitLotsArgs args);
}
