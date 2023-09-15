using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Client.Services.Units.Return;

/// <summary>
/// API service provides methods to retrieve/handle unit return data.
/// </summary>
public interface IUnitReturnApiService
{
    /// <summary>
    /// Retrieves unit return details.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <returns>Unit return details.</returns>
    Task<UnitReturnDetailsDto> GetReturnDetailsAsync(int unitKeyId);

    /// <summary>
    /// Returns unit.
    /// </summary>
    /// <param name="args">Unit return arguments.</param>
    /// <returns>Unit key identifier.</returns>
    Task<int> ReturnAsync(UnitReturnArgs args);
}