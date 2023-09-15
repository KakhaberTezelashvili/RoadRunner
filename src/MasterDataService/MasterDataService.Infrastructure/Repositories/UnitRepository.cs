namespace MasterDataService.Infrastructure.Repositories;

/// <inheritdoc cref="IUnitRepository" />
public class UnitRepository : RepositoryBase<UnitModel>, IUnitRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public UnitRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<UnitModel?> GetByKeyIdAsync(int keyId) =>
        await _context.Units.AsNoTracking().Where(p => p.KeyId == keyId).FirstOrDefaultAsync();
}