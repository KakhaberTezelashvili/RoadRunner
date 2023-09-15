using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Client.Services.Units.LotInfo;

/// <summary>
/// Service provides methods to retrieve/handle unit lot data.
/// </summary>
public interface IUnitLotInfoApiService
{
    /// <summary>
    /// Updates unit lots.
    /// </summary>
    /// <param name="args">Unit lots arguments.</param>
    Task UpdateLotsAsync(UnitLotsArgs args);
}