namespace MasterDataService.Core.Services.Items.Recent;

/// <summary>
/// Service provides methods to retrieve/handle item recent data.
/// </summary>
public interface IItemRecentService
{
    /// <summary>
    /// Retrieves the most recent item.
    /// </summary>
    /// <returns>Item.</returns>
    Task<ItemModel?> GetRecentAsync();
}
