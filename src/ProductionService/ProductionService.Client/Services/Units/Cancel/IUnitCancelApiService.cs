using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Client.Services.Units.Cancel;

/// <summary>
/// API service provides methods to retrieve/handle unit cancel data.
/// </summary>
public interface IUnitCancelApiService
{
    /// <summary>
    /// Cancels unit(s).
    /// </summary>
    /// <param name="args">Unit cancel arguments.</param>
    Task CancelAsync(UnitCancelArgs args);
}