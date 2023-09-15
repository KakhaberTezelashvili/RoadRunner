using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Client.Services.Units.Pack;

/// <summary>
/// API service provides methods to retrieve/handle unit pack data.
/// </summary>
public interface IUnitPackApiService
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
    /// <param name="args">Unit pack arguments.</param>
    /// <returns>Collection of unit key identifiers.</returns>
    Task<IList<int>> PackAsync(UnitPackArgs args);
}