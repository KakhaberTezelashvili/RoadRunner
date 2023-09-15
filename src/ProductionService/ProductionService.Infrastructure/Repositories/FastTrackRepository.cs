using ProductionService.Core.Models.FastTracking;

namespace ProductionService.Infrastructure.Repositories;

/// <inheritdoc cref="IFastTrackRepository" />
public class FastTrackRepository : RepositoryBase<UnitFastTrackModel>, IFastTrackRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FastTrackRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public FastTrackRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IList<UnitFastTrackData>> GetDisplayInfoByUnitKeyIdAsync(int unitKeyId)
    {
        // get all FT code IDs and plan IDs assigned to the unit
        return await _context.UnitFastTracking
            .Where(ft => ft.UnitUnit == unitKeyId && ft.Status != FastTrackStatus.Ended)
            .Select(ft => new UnitFastTrackData
            {
                AutoInc = ft.AutoInc,
                Status = ft.Status,
                PlanKeyId = ft.FTPlKeyId,
                CodeKeyId = ft.FTCoKeyId,
                Id = (ft.FTCo.Code ?? ft.FTPl.Plan),
                Name = (ft.FTCo.Name ?? ft.FTPl.Name),
                Priority = ft.FTCoKeyId == null ? ft.FTPl.Priority : ft.FTCo.Priority,
                DisplayMode = (ft.FTCo.DisplayMode ?? ft.FTPl.DisplayMode),
                IsPlanActive = ft.FTPlKeyId != null && _context.UnitFastTrackPlan
                    .Any(pl => pl.UFTAutoInc == ft.AutoInc && pl.EndTime == null)
            })
            .ToListAsync();
    }
}