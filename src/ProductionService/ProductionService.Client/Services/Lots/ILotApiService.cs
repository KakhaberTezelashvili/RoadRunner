using ProductionService.Shared.Dtos.Lots;

namespace ProductionService.Client.Services.Lots;

/// <summary>
/// API service provides methods to retrieve/handle lots.
/// </summary>
public interface ILotApiService
{
    /// <summary>
    /// Retrieves unit lot information (linked and avialable for linking lot numbers).
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <returns>The unit details for pack workflow are returned.</returns>
    Task<LotInformationDto> GetUnitLotInformationAsync(int unitKeyId);
}