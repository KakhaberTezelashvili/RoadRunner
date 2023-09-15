namespace MasterDataService.Core.Repositories.Interfaces.Items.Recent;

/// <summary>
/// Repository provides methods to retrieve/handle item recent data.
/// </summary>
public interface IItemRecentRepository : IRepositoryBase<ItemModel>
{
    /// <summary>
    /// Retrieves the last item.
    /// </summary>
    /// <returns>Item.</returns>
    Task<ItemModel?> GetLastAsync();
}