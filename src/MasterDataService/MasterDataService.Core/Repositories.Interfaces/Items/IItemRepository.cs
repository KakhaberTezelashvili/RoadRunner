namespace MasterDataService.Core.Repositories.Interfaces.Items;

/// <summary>
/// Repository provides methods to retrieve/handle items.
/// </summary>
public interface IItemRepository : IRepositoryBase<ItemModel>
{
    /// <summary>
    /// Adds a new item.
    /// </summary>
    /// <param name="data">Data for a new item.</param>
    /// <returns>New item key identifier.</returns>
    Task<int> AddDataAsync(ItemModel data);

    /// <summary>
    /// Retrieves item by key identifier.
    /// </summary>
    /// <param name="keyId">Item key identifier.</param>
    /// <returns>Item.</returns>
    Task<ItemModel?> GetByKeyIdAsync(int keyId);

    /// <summary>
    /// Retrieves item by number/identifier.
    /// </summary>
    /// <param name="item">The number/identifier of the item.</param>
    /// <returns>Item.</returns>
    Task<ItemModel?> GetByItemAsync(string item); 
}