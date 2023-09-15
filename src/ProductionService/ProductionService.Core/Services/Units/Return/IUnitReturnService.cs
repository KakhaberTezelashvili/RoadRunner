using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Return;

/// <summary>
/// Service provides methods to retrieve/handle unit return data.
/// </summary>
public interface IUnitReturnService
{
    /// <summary>
    /// Retrieves unit return details.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <returns>Unit pack details.</returns>
    Task<UnitReturnDetailsDto> GetReturnDetailsAsync(int unitKeyId);

    /// <summary>
    /// Returns unit.
    /// </summary>
    /// <param name="userKeyId">User key identifier.</param>
    /// <param name="args">Unit return arguments.</param>
    /// <returns>Unit key identifier.</returns>
    Task<int> ReturnAsync(int userKeyId, UnitReturnArgs args);
}