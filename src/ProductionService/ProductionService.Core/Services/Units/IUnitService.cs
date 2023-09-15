using ProductionService.Core.Models.Units;

namespace ProductionService.Core.Services.Units;

/// <summary>
/// Service provides methods to retrieve/handle units.
/// </summary>
public interface IUnitService
{
    /// <summary>
    /// Updates unit status or error.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <param name="status">Unit status.</param>
    /// <param name="error">Error.</param>
    /// <returns></returns>
    Task UpdateUnitStatusOrErrorAsync(int unitKeyId, int? status = null, int? error = null);

    /// <summary>
    /// Retrieves list of units by key identifiers.
    /// </summary>
    /// <param name="unitKeyIds">List of unit key identifiers.</param>
    /// <returns>List of unit data.</returns>
    Task<IList<UnitModel>> GetByKeyIdsAsync(IEnumerable<int> unitKeyIds);
}