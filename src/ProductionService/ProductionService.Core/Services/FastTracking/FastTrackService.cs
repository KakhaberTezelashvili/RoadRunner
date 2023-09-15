using ProductionService.Core.Models.FastTracking;

namespace ProductionService.Core.Services.FastTracking;

/// <inheritdoc cref="IFastTrackService" />
public class FastTrackService : IFastTrackService
{
    private readonly IFastTrackRepository _fastTrackRepository;
    private readonly IFastTrackValidator _fastTrackValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="FastTrackService" /> class.
    /// </summary>
    /// <param name="fastTrackRepository">Repository provides methods to retrieve/handle fast track data.</param>
    /// <param name="fastTrackValidator">Validator provides methods to validate fast track data.</param>
    public FastTrackService(IFastTrackRepository fastTrackRepository, IFastTrackValidator fastTrackValidator)
    {
        _fastTrackRepository = fastTrackRepository;
        _fastTrackValidator = fastTrackValidator;
    }

    /// <inheritdoc />
    public async Task<FastTrackDisplayInfo> GetUnitFastTrackDisplayInfoAsync(int unitKeyId)
    {
        _fastTrackValidator.UnitFastTrackDisplayInfoValidate(unitKeyId);
        IList<UnitFastTrackData> unitFastTracks = await _fastTrackRepository.GetDisplayInfoByUnitKeyIdAsync(unitKeyId);
        if (unitFastTracks.Count == 0)
            return null;
        var result = new FastTrackDisplayInfo();
        foreach (UnitFastTrackData ftInfo in unitFastTracks.Where(ftInfo => ftInfo.Status != FastTrackStatus.Assigned))
        {
            if (ftInfo.PlanKeyId != null && ftInfo.IsPlanActive) // FT plan assigned
            {
                result.AddFastTrackByPlan((int)ftInfo.PlanKeyId, ftInfo.Id, ftInfo.Name,
                    ftInfo.Priority, ftInfo.DisplayMode);
            }
            else if (ftInfo.PlanKeyId == null) // ft code assigned
            {
                result.AddFastTrackByCode((int)ftInfo.CodeKeyId, ftInfo.Id, ftInfo.Name, ftInfo.Priority,
                    ftInfo.DisplayMode);
            }
        }
        return result;
    }
}