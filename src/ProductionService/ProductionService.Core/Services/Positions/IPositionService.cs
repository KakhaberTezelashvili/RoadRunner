using ProductionService.Shared.Dtos.Positions;

namespace ProductionService.Core.Services.Positions;

/// <summary>
/// Service provides methods to retrieve/handle positions.
/// </summary>
public interface IPositionService
{
    /// <summary>
    /// Retrieves a position and all locations linked it.
    /// </summary>
    /// <param name="positionKeyId">Primary key of the position.</param>
    /// <returns>A <see cref="Result"/> instance indicating the result of the operation.
    /// It will contain a <see cref="PositionModel" /> instance including all linked locations, if the position was found;
    /// <c>null</c> otherwise.</returns>
    Task<PositionModel> GetPositionLocationsAsync(int positionKeyId);

    /// <summary>
    /// Retrieves all locations linked to the specified position.
    /// </summary>
    /// <param name="positionKeyId">Primary key of the position.</param>
    /// <returns>A <see cref="Result"/> instance indicating the result of the operation.
    /// It will contain a list of <see cref="PositionLocationsDetailsDto" />, if the position was found;
    /// <c>null</c> otherwise.</returns>
    Task<IList<PositionLocationsDetailsDto>> GetScannerLocationsAsync(int positionKeyId);
}