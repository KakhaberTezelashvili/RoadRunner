namespace MasterDataService.Client.Services.DesktopData;

/// <summary>
/// API service provides methods to retrieve/handle desktop data.
/// </summary>
public interface IDesktopDataApiService
{
    /// <summary>
    /// Get component state from the database.
    /// </summary>
    /// <param name="identifier">Identifier of component.</param>
    /// <returns>Component state.</returns>
    Task<string> GetComponentStateAsync(string identifier);

    /// <summary>
    /// Creates or updates component state in database based on identifier.
    /// </summary>
    /// <param name="identifier">Identifier of component.</param>
    /// <param name="data">Data of component.</param>
    /// <returns></returns>
    Task SetComponentStateAsync(string identifier, string data);
}