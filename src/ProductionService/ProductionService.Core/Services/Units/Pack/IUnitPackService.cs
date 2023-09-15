using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Pack;

/// <summary>
/// Service provides methods to retrieve/handle unit pack data.
/// </summary>
public interface IUnitPackService
{
    /// <summary>
    /// Retrieves unit pack details.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <returns>Unit pack details.</returns>
    Task<UnitPackDetailsDto> GetPackDetailsAsync(int unitKeyId);

    /// <summary>
    /// Packs new unit(s).
    /// </summary>
    /// <param name="userKeyId">User key identifier.</param>
    /// <param name="args">Unit pack arguments.</param>
    /// <returns>Collection of unit key identifiers.</returns>
    Task<IEnumerable<int>> PackAsync(int userKeyId, UnitPackArgs args);
}