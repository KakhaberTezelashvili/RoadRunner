using ProductionService.Shared.Dtos.Positions;

namespace ProductionService.Infrastructure.Repositories;

/// <inheritdoc cref="IPositionRepository" />
public class PositionRepository : RepositoryBase<PositionModel>, IPositionRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PositionRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public PositionRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<PositionModel> GetWithLocationsByKeyIdAsync(int keyId)
    {
        PositionModel position = await EntitySet.AsNoTracking()
            .Include(p => p.PosPosLocationList)
            .ThenInclude(pl => pl.Loca)
            .SingleOrDefaultAsync(p => p.KeyId == keyId);

        position?.PosPosLocationList?.Sort((x, y) => string.Compare(x.Loca.Name, y.Loca.Name, StringComparison.Ordinal));
        return position;
    }

    /// <inheritdoc />
    public async Task<IList<PositionLocationsDetailsDto>> GetScannerLocationsByKeyIdAsync(int keyId)
    {
        ProcessType[] allowedLocations = new[]
        {
            ProcessType.ReturnWF, ProcessType.WashPreBatchWF, ProcessType.WashPostBatchWF, ProcessType.PackWF,
            ProcessType.SteriPreBatchWF, ProcessType.SteriPostBatchWF, ProcessType.OutWF,
        };

        return (await _context.PositionLocations.AsNoTracking()
                .Where(pl => pl.PosKeyId == keyId & allowedLocations.Contains(pl.Loca.Process) & pl.Loca.Status == 10)
                .Select(l => new PositionLocationsDetailsDto
                {
                    PositionLocationKeyId = l.KeyId,
                    LocationKeyId = l.Loca.KeyId,
                    LocationName = l.Loca.Name,
                    FactoryKeyId = (int)l.Loca.FacKeyId,
                    Process = l.Loca.Process,
                    UIAvailability = l.UIAvailability != UILocaAvailability.Default
                        ? l.Loca.UIAvailability
                        : l.UIAvailability,
                    Default = l.Default,
                    ShowMES = l.Loca.LocaMesTaskLocationsList.Count > 0
                })
                .ToListAsync()
            )
            .OrderBy(entry => Array.IndexOf(allowedLocations, entry.Process))
            .ToList();
    }
}