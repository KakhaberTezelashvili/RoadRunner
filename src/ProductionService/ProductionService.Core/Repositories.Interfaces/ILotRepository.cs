using ProductionService.Core.Models.Lots;
using ProductionService.Core.Services.Lots;
using ProductionService.Shared.Dtos.Lots;

namespace ProductionService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle lots.
/// </summary>
public interface ILotRepository : IRepositoryBase<LotInfoModel>
{
    /// <summary>
    /// Updates supported lot numbers list with all active and not expired lots excluding linked lots
    /// </summary>
    /// <param name="supportedItems">List of supported lots.</param>
    /// <param name="excludedLots">List of excluded lots.</param>
    /// <param name="lotInformation">Lot information.</param>
    /// <returns></returns>
    Task ItemSupportedLotNumbersAsync(IList<int> supportedItems, IList<int> excludedLots, LotInformationDto lotInformation);

    /// <summary>
    /// Retrieves list of lot information entries for the specified entity.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <param name="hasLinkToPositionField">Lot information has link to "Position" field.</param>
    /// <param name="hasBoundArticleField">Lot information has link to "Bound articles" field.</param>
    /// <returns>List of <see cref="LotInformationEntryDto"/></returns>
    Task<IList<LotInformationEntryDto>> GetLotInfoEntriesByUnitKeyIdAsync(int unitKeyId, bool hasLinkToPositionField, bool hasBoundArticleField);

    /// <summary>
    /// Checks if an operation is assigned to the entity.
    /// </summary>
    /// <param name="entityKeyId">Entity key id.</param>
    /// <returns><c>true</c> if an operation is assigned to the entity; <c>false</c> otherwise.</returns>
    Task<bool> IsOperationAssignedAsync(int entityKeyId);

    /// <summary>
    /// Retrieves list of item lot information for the specified entity.
    /// </summary>
    /// <param name="lotParams">lot information parameters.</param>
    /// <param name="includeItems">List of item key ids need to be included.</param>
    /// <returns>List of <see cref="ItemLotInformationExtDto"/></returns>
    Task<Tuple<IList<ItemLotInformationExtDto>, bool>> GetItemLotInfoListAsync(LotInformationParams lotParams, IList<int> includeItems);

    /// <summary>
    /// Retrieves and returns lot header information based on lot parameters.
    /// </summary>
    /// <param name="lotParams">Lot information parameters.</param>
    /// <param name="isComposite">Lot entity is based on a composite item.</param>
    /// <returns><see cref="HeaderInfo"/></returns>
    Task<HeaderInfo> GetHeaderInfoByLotParamsAsync(LotInformationParams lotParams, bool isComposite);

    /// <summary>
    /// Retrieves list of matched lots.
    /// </summary>
    /// <param name="lotInformation">Lot information.</param>
    /// <returns>List of matched lots key identifiers.</returns>
    Task<IList<int>> GetMatchedLotIdsAsync(LotInformationDto lotInformation);

    /// <summary>
    /// Retrieves list of unit content.
    /// </summary>
    /// <param name="lotInformation">Lot information.</param>
    /// <returns>List of unit content list item key identifiers.</returns>
    Task<IList<int?>> GetUnitContentListAsync(LotInformationDto lotInformation);
}