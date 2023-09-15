using MasterDataService.Core.Repositories.Interfaces.Items;

namespace MasterDataService.Infrastructure.Repositories.Items;

/// <inheritdoc cref="IItemRepository" />
public class ItemRepository : RepositoryBase<ItemModel>, IItemRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public ItemRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<int> AddDataAsync(ItemModel data)
    {
        _context.Items.Add(data);
        await _context.SaveChangesAsync();
        return data.KeyId;
    }

    /// <inheritdoc />
    public async Task<ItemModel?> GetByKeyIdAsync(int keyId)
    {
        return await _context.Items
            .Include(i => i.CreatedUser)
            .Include(i => i.ModifiedUser)
            .Include(i => i.ItGrp)
            .Include(i => i.Supp)
            .Include(i => i.Manu)
            .Where(i => i.KeyId == keyId)
            .AsNoTracking().FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<ItemModel?> GetByItemAsync(string item)
    {
        return await _context.Items
            .Where(i => i.Item == item)
            .AsNoTracking().FirstOrDefaultAsync();
    }
}