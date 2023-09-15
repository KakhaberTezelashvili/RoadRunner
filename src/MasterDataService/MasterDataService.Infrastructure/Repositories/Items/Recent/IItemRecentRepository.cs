using MasterDataService.Core.Repositories.Interfaces.Items.Recent;

namespace MasterDataService.Infrastructure.Repositories.Items.Recent;

/// <inheritdoc cref="IItemRecentRepository" />
public class ItemRecentRepository : RepositoryBase<ItemModel>, IItemRecentRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemRecentRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public ItemRecentRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ItemModel?> GetLastAsync()
    {
        return await EntitySet
            .Include(i => i.CreatedUser)
            .Include(i => i.ModifiedUser)
            .Include(i => i.ItGrp)
            .Include(i => i.Supp)
            .Include(i => i.Manu)
            .AsNoTracking().OrderBy(i => i.KeyId).LastOrDefaultAsync();
    }
}