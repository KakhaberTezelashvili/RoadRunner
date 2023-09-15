namespace MasterDataService.Core.Repositories.Interfaces;

/// <summary>
/// TODO: this is duplication with ProductionService.
/// Repository provides methods to retrieve/handle units.
/// </summary>
public interface IUnitRepository : IRepositoryBase<UnitModel>
{
    /// <summary>
    /// Retrieves unit by key identifier.
    /// </summary>
    /// <param name="keyId">Unit key identifier.</param>
    /// <returns>Unit.</returns>
    Task<UnitModel?> GetByKeyIdAsync(int keyId);
}
