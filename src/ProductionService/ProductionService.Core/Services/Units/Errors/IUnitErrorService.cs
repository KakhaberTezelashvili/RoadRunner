using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Errors;

/// <summary>
/// Service provides methods to retrieve/handle unit error data.
/// </summary>
public interface IUnitErrorService
{
    /// <summary>
    /// Updates the unit with the specified error.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <param name="args">Unit error arguments.</param>
    Task UpdateErrorAsync(int unitKeyId, UnitErrorArgs args);
}