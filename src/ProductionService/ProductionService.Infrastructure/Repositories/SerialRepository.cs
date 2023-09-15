namespace ProductionService.Infrastructure.Repositories;

/// <inheritdoc cref="ISerialRepository" />
public class SerialRepository : RepositoryBase<SerialModel>, ISerialRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SerialRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public SerialRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<SerialModel> GetByKeyIdAsync(int keyId)
    {
        return await _context.SerialNumbers.AsNoTracking()
            .Where(sn => sn.KeyId == keyId)
            .FirstOrDefaultAsync();
    }
}