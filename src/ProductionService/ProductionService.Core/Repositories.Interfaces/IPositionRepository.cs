using ProductionService.Shared.Dtos.Positions;

namespace ProductionService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle positions.
/// </summary>
public interface IPositionRepository : IRepositoryBase<PositionModel>
{
    /// <summary>
    /// Retrieves the specified position and all locations linked to it.
    /// </summary>
    /// <param name="keyId">Position key identifier.</param>
    /// <returns>
    /// A <see cref="PositionModel"/> instance including all linked locations, if the position
    /// was found; <c>null</c> otherwise.
    /// </returns>
    Task<PositionModel> GetWithLocationsByKeyIdAsync(int keyId);

    /// <summary>
    /// Retrieves all locations linked to the specified position.
    /// </summary>
    /// <param name="keyId">Position key identifier.</param>
    /// <returns>
    /// A list of <see cref="PositionLocationsDetailsDto"/>, if the position was found;
    /// <c>empty</c> otherwise.
    /// </returns>
    Task<IList<PositionLocationsDetailsDto>> GetScannerLocationsByKeyIdAsync(int keyId);
}