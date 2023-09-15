using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Repositories.Interfaces.Units;

/// <summary>
/// Repository provides methods to retrieve/handle unit return data.
/// </summary>
public interface IUnitReturnRepository : IRepositoryBase<UnitModel>
{
    /// <summary>
    /// Retrieves unit return details.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <returns>Unit return details.</returns>
    Task<UnitReturnDetailsDto> GetReturnDetailsAsync(int unitKeyId);
}