using ProductionService.Core.Models.Lots;
using ProductionService.Shared.Dtos.Lots;

namespace ProductionService.Core.Services.Lots;

/// <summary>
/// Service provides methods to retrieve/handle lots.
/// </summary>
public interface ILotService
{
    /// <summary>
    /// Retrieves lot information of specified entity.
    /// </summary>
    /// <param name="lotParams">Lot information parameters.</param>
    /// <returns>A <see cref="LotInformationDto"/>.</returns>
    Task<LotInformationDto> GetLotInformationAsync(LotInformationParams lotParams);

    /// <summary>
    /// Updates lot information of specified entity.
    /// </summary>
    /// <param name="lotInformation">Lot information.</param>
    Task UpdateLotInformationAsync(LotInformationDto lotInformation);
}