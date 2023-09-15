using ProductionService.Core.Models.Units;
using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Pack;

/// <summary>
/// Validator provides methods to validate unit pack data.
/// </summary>
public interface IUnitPackValidator
{
    /// <summary>
    /// Validates packing new unit(s).
    /// </summary>
    /// <param name="args">Unit pack arguments.</param>
    /// <returns>If the validation passes, unit data to pack will be returned.</returns>
    Task<UnitDataToPack> PackValidateAsync(UnitPackArgs args);
}