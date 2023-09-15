using ProductionService.Shared.Dtos.Positions;

namespace ProductionService.Client.Services.Positions;

/// <summary>
/// API service provides methods to retrieve/handle positions.
/// </summary>
public interface IPositionApiService
{
    /// <summary>
    /// Retrieves all locations linked to the specified position.
    /// </summary>
    /// <param name="positionKeyId">Position key identifier.</param>
    /// <returns>The list of position locations details are returned.</returns>
    Task<List<PositionLocationsDetailsDto>> GetWorkflowsByPositionKeyIdAsync(int positionKeyId);
}