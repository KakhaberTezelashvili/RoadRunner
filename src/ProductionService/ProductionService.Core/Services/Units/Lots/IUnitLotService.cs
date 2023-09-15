using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Lots;

/// <summary>
/// Service provides methods to retrieve/handle unit lot data.
/// </summary>
public interface IUnitLotService
{
    /// <summary>
    /// Updates unit lots.
    /// </summary>
    /// <param name="args">Unit lots arguments.</param>
    Task UpdateLotsAsync(UnitLotsArgs args);
}
