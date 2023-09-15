namespace MasterDataService.Core.Services.DesktopData;

/// <summary>
/// Service provides methods to retrieve/handle desktop data.
/// </summary>
public interface IDesktopDataService
{
    /// <summary>
    /// Get component desktop state from the database.
    /// </summary>
    /// <param name="userKeyId">User key identifier.</param>
    /// <param name="identifier">Component identifier.</param>
    /// <returns>Desktop model</returns>
    Task<DesktopModel> GetComponentStateAsync(int userKeyId, string identifier);

    /// <summary>
    /// Creates or updates component state in database based on identifier.
    /// </summary>
    /// <param name="userKeyId">User key identifier.</param>
    /// <param name="identifier">Component identifier.</param>
    /// <param name="data">Component data.</param>
    /// <returns></returns>
    Task SetComponentStateAsync(int userKeyId, string identifier, string data);
}