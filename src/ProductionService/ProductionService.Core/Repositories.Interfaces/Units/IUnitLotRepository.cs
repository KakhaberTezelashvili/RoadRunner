namespace ProductionService.Core.Repositories.Interfaces.Units;

/// <summary>
/// Repository provides methods to retrieve/handle unit lot data.
/// </summary>
public interface IUnitLotRepository : IRepositoryBase<UnitLotInfoModel>
{
    /// <summary>
    /// Finds unit lot information by unit key identifier.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <returns>Collection of unit lot information.</returns>
    Task<IList<UnitLotInfoModel>> FindByUnitKeyIdAsync(int unitKeyId);
}
