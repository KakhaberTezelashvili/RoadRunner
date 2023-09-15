using ProductionService.Core.Models.Units;
using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Repositories.Interfaces.Units;

/// <summary>
/// Repository provides methods to retrieve/handle unit pack data.
/// </summary>
public interface IUnitPackRepository : IRepositoryBase<UnitModel>
{
    /// <summary>
    /// Retrieves unit pack details.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <returns>Unit pack details.</returns>
    Task<UnitPackDetailsDto> GetPackDetailsAsync(int unitKeyId);

    /// <summary>
    /// Retrieves additional data required to pack a new unit.
    /// </summary>
    /// <param name="args">Unit pack arguments.</param>
    /// <returns>Unit data to pack.</returns>
    Task<UnitDataToPack> GetDataToPackAsync(UnitPackArgs args);

    /// <summary>
    /// Add packed list of items for the unit.
    /// </summary>
    /// <param name="data">Unit data.</param>
    /// <param name="locationKeyId">Location key identifier.</param>
    /// <param name="positionLocationKeyId">Position/Location key identifier.</param>
    Task AddPackedListOfItemsAsync(UnitModel data, int locationKeyId, int positionLocationKeyId);
}