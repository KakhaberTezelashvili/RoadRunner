using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Dispatch;

/// <summary>
/// Service provides methods to retrieve/handle unit dispatch data.
/// </summary>
public interface IUnitDispatchService
{
    /// <summary>
    /// Dispatches unit(s).
    /// </summary>    
    /// <param name="userKeyId">User key identifier.</param>
    /// <param name="args">Unit dispatch arguments.</param>
    Task DispatchAsync(int userKeyId, UnitDispatchArgs args);
}