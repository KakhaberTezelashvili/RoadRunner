using ProductionService.Core.Models.Lots;
using ProductionService.Shared.Dtos.Lots;

namespace ProductionService.Core.Services.Lots;

/// <summary>
/// Validator provides methods to validate lots.
/// </summary>
public interface ILotValidator
{
    /// <summary>
    /// Validates lot information parameters.
    /// </summary>
    /// <param name="lotParams">Input parameters needed for retrieving the lot information.</param>
    void LotInformationParamsValidate(LotInformationParams lotParams);

    /// <summary>
    /// Validates lot information.
    /// </summary>
    /// <param name="lotInformation">Lot information.</param>
    Task LotInformationValidateAsync(LotInformationDto lotInformation);
}