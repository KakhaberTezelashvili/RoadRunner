using MasterDataService.Core.Repositories.Interfaces.Items.Recent;

namespace MasterDataService.Core.Services.Items.Recent;

/// <inheritdoc cref="IItemRecentService" />
public class ItemRecentService : IItemRecentService
{
    private readonly IItemRecentRepository _itemRecentRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemRecentService" /> class.
    /// </summary>
    /// <param name="itemRecentRepository">Repository provides methods to retrieve/handle item recent data.</param>
    public ItemRecentService(IItemRecentRepository itemRecentRepository)
    {
        _itemRecentRepository = itemRecentRepository;
    }

    /// <inheritdoc />
    public async Task<ItemModel?> GetRecentAsync() => await _itemRecentRepository.GetLastAsync();
}