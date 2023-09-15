using MasterDataService.Shared.Dtos.Items;

namespace MasterDataService.Core.Services.Items;

/// <summary>
/// Service provides methods to retrieve/handle items.
/// </summary>
public interface IItemService
{
    /// <summary>
    /// Adds a new item.
    /// </summary>
    /// <param name="args">Item add arguments.</param>
    /// <returns>Item key identifier.</returns>
    Task<int> AddDataAsync(ItemAddArgs args);

    /// <summary>
    /// Retrieves item by key identifier.
    /// </summary>
    /// <param name="keyId">Item key identifier.</param>
    /// <returns>Item.</returns>
    Task<ItemModel> GetByKeyIdAsync(int keyId);

    /// <summary>
    /// Updates the item.
    /// </summary>
    /// <param name="args">Item update arguments.</param>
    Task UpdateDataAsync(ItemUpdateArgs args);
}