using ProductionService.Core.Models.FastTracking;

namespace ProductionService.Core.Services.FastTracking;

/// <summary>
/// Service provides methods to retrieve/handle fast track data.
/// </summary>
public interface IFastTrackService
{
    /// <summary>
    /// Retrieves unit fast track info for CountAssure.
    /// </summary>
    /// <param name="unitKeyId">Unit key ID</param>
    /// <returns>Fast tracking info for the unit.</returns>
    Task<FastTrackDisplayInfo> GetUnitFastTrackDisplayInfoAsync(int unitKeyId);
}