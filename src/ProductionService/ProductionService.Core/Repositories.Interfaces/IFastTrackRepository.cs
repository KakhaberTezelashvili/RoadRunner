using ProductionService.Core.Models.FastTracking;

namespace ProductionService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle fast track data.
/// </summary>
public interface IFastTrackRepository : IRepositoryBase<UnitFastTrackModel>
{
    /// <summary>
    /// Retrieves fast track info for the unit.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <returns>UnitFastTrackData model.</returns>
    public Task<IList<UnitFastTrackData>> GetDisplayInfoByUnitKeyIdAsync(int unitKeyId);
}