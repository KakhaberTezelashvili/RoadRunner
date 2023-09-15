using MasterDataService.Shared.Dtos.Items;

namespace MasterDataService.Client.Services.Items.Recent;

/// <summary>
/// API service provides methods to retrieve/handle item recent data.
/// </summary>
public interface IItemRecentApiService
{
    /// <summary>
    /// Retrieves the most recent item details.
    /// </summary>
    /// <returns>Item details.</returns>
    Task<ItemDetailsDto> GetRecentAsync();
}