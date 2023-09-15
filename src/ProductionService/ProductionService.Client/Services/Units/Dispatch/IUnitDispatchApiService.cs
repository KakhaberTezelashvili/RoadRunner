using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Client.Services.Units.Dispatch;

/// <summary>
/// API service provides methods to retrieve/handle unit dispatch data.
/// </summary>
public interface IUnitDispatchApiService
{
    /// <summary>
    /// Dispatches unit(s).
    /// </summary>
    /// <param name="args">Dispatch unit arguments.</param>
    Task DispatchAsync(UnitDispatchArgs args);
}