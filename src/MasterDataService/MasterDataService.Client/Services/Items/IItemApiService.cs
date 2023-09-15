using MasterDataService.Shared.Dtos.Items;

namespace MasterDataService.Client.Services.Items;

/// <summary>
/// API service provides methods to retrieve/handle items.
/// </summary>
public interface IItemApiService
{
    /// <summary>
    /// Adds a new item.
    /// </summary>
    /// <param name="args">Item add arguments.</param>
    /// <returns>Item key identifier.</returns>
    Task<int> AddDataAsync(ItemAddArgs args);

    /// <summary>
    /// Retrieves item details by key identifier.
    /// </summary>
    /// <param name="keyId">Item key identifier.</param>
    /// <returns>Item details.</returns>
    Task<ItemDetailsDto> GetByKeyIdAsync(int keyId);

    /// <summary>
    /// Updates the item.
    /// </summary>
    /// <param name="args">Item update arguments.</param>
    Task UpdateDataAsync(ItemUpdateArgs args);
}