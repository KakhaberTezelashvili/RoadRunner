using ProductionService.Core.Models.Units;

namespace ProductionService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle units.
/// </summary>
public interface IUnitRepository : IRepositoryBase<UnitModel>
{
    /// <summary>
    /// Adds a new unit.
    /// </summary>
    /// <param name="data">Data for a new unit.</param>
    /// <returns>New unit key identifier.</returns>
    Task<int> AddDataAsync(UnitModel data);

    /// <summary>
    /// Updates unit related data for a product serial after new unit is packed.
    /// </summary>
    /// <param name="productSerialKeyId">Product serial key identifier.</param>
    /// <param name="usageCount">Usage count.</param>
    /// <param name="unitKeyId">Unit key identifier.</param>
    Task UpdateUnitDataForProductSerialAsync(int productSerialKeyId, int usageCount, int unitKeyId);

    /// <summary>
    /// Sets NextUnit value for the 'previous' unit after 'next' unit is packed.
    /// </summary>
    /// <param name="unitKeyId">Unit to be updated.</param>
    /// <param name="nextUnitKeyId">NextUnit value to be set.</param>
    Task SetUnitNextUnitAsync(int unitKeyId, int nextUnitKeyId);

    /// <summary>
    /// Adds a new record into Unit location (TUNITLCA) table.
    /// </summary>
    /// <param name="unitLocationData">
    /// Struct that keeps all the values needed for insertion a new record into Unit location table.
    /// </param>
    Task AddUnitLocationRecordAsync(UnitLocationData unitLocationData);

    /// <summary>
    /// Retrieves unit by key identifier.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <returns>Unit.</returns>
    Task<UnitModel> GetUnitAsync(int unitKeyId);

    /// <summary>
    /// Updates the unit with the specified operation.
    /// </summary>
    /// <param name="unit">Unit.</param>
    /// <param name="operationKeyId">Operation key identifier.</param>
    Task UpdateOperationAsync(UnitModel unit, int? operationKeyId);

    /// <summary>
    /// Retrieves details related to the specified unit; these include product details, item
    /// details, fast track information and weight information.
    /// </summary>
    /// <param name="unitKeyId">Primary key of the unit.</param>
    /// <returns>A <see cref="UnitWeight"/> instance if the unit was found; otherwise, <c>null</c>.</returns>
    Task<UnitWeight> GetUnitWeightAsync(int unitKeyId);

    /// <summary>
    /// Retrives all fast track codes.
    /// </summary>
    /// <returns>Collection of fast track codes.</returns>
    Task<IList<FastTrackCodeModel>> GetFastTrackCodesAsync();

    /// <summary>
    /// Get unit full fast track info.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <returns>
    /// Two parameters:
    /// - all fast track codes assigned to the Unit.
    /// - all fast track plans assigned to the Unit.
    /// </returns>
    Task<(IList<string>, IList<string>)> GetUnitFastTrackInfoAsync(int unitKeyId);

    /// <summary>
    /// Retrieves details related to the specified unit; these include product, item, location,
    /// room, serial number and stock details.
    /// </summary>
    /// <param name="unitKeyId">Primary key of the unit.</param>
    /// <returns>Unit.</returns>
    Task<UnitModel> GetUnitDetailsAsync(int unitKeyId);

    /// <summary>
    /// Retrieves list of units containing part of or whole primary key of the unit.
    /// </summary>
    /// <param name="unitKeyId">Part of or whole primary key of the unit.</param>
    /// <returns>Collection of units.</returns>
    Task<IList<UnitModel>> SearchByUnitAsync(int unitKeyId);

    /// <summary>
    /// Checks if unit has assigned requested fast track code.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <param name="fastTrackCodeKeyId">Unit fast track code which needs to be canceled.</param>
    /// <returns>Response will indicate if unit has assigned requested fast track code.</returns>
    Task<bool> UnitHasFastTrackCodeAsync(int unitKeyId, int fastTrackCodeKeyId);

    /// <summary>
    /// Get Unit event list.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <param name="startingRow">Starting row number.</param>
    /// <param name="pageRowCount">Page row count.</param>
    /// <param name="sortPropertyName">Sort property name.</param>
    /// <param name="sortDescending">Sort in descending order.</param>
    /// <returns>Unit event list and total row count.</returns>
    Task<(IList<EventModel>, int)> GetUnitEventListAsync(int unitKeyId, int startingRow, int pageRowCount, string sortPropertyName, bool sortDescending);

    /// <summary>
    /// Retrieves unit statuses for units.
    /// </summary>
    /// <param name="unitKeyIds">List of unit key identifiers.</param>
    /// <returns>Collection of <see cref="UnitStatusInfo"/>.</returns>
    Task<IList<UnitStatusInfo>> GetUnitStatusesByKeyIdsAsync(IEnumerable<int> unitKeyIds);

    /// <summary>
    /// Retrieves list of units with products and items by key identifiers.
    /// </summary>
    /// <param name="unitKeyIds">List of unit key identifiers.</param>
    /// <returns>Collection of units.</returns>
    Task<IList<UnitModel>> GetWithProductAndItemByKeyIdsAsync(IEnumerable<int> unitKeyIds);

    /// <summary>
    /// Retrieves collection of units with products and items by serial key identifiers.
    /// </summary>
    /// <param name="serialKeyIds">List of serial key identifiers.</param>
    /// <returns>Collection of units.</returns>
    Task<IList<UnitModel>> GetWithProductAndItemBySerialKeyIdsAsync(IEnumerable<int?> serialKeyIds);

    /// <summary>
    /// Retrieves list of units by key identifiers.
    /// </summary>
    /// <param name="unitKeyIds">List of unit key identifiers.</param>
    /// <returns>Collection of units.</returns>
    Task<IList<UnitModel>> GetByKeyIdsAsync(IEnumerable<int> unitKeyIds);
}