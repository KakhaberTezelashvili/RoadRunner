using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Cancel;

/// <summary>
/// Service provides methods to retrieve/handle unit cancel data.
/// </summary>
public interface IUnitCancelService
{
    /// <summary>
    /// Cancels unit(s).
    /// </summary>
    /// <param name="args">Unit cancel arguments.</param>
    Task CancelAsync(UnitCancelArgs args);
}