using ProductionService.Core.Repositories.Interfaces.Units;

namespace ProductionService.Infrastructure.Repositories.Units;

/// <inheritdoc cref="IUnitLotRepository" />
public class UnitLotRepository : RepositoryBase<UnitLotInfoModel>, IUnitLotRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitLotRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public UnitLotRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IList<UnitLotInfoModel>> FindByUnitKeyIdAsync(int unitKeyId) => await _context.UnitLotInfo.Where(uli => uli.UnitUnit == unitKeyId).ToListAsync();
}