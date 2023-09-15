namespace MasterDataService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle desktop data.
/// </summary>
public interface IDesktopDataRepository : IRepositoryBase<DesktopModel>
{
    /// <summary>
    /// Get component desktop state from the database.
    /// </summary>
    /// <param name="userKeyId">User key identifier.</param>
    /// <param name="dataIdentifier">Identifier of component.</param>
    /// <returns>Desktop model.</returns>
    Task<DesktopModel> GetByUserKeyIdAndDataIdentifierAsync(int userKeyId, string dataIdentifier);

    /// <summary>
    /// Get Web server desktop options from the database.
    /// </summary>
    /// <returns>Desktop model.</returns>
    Task<DesktopModel> GetWebServerDesktopOptionsAsync();

    /// <summary>
    /// Get Web user desktop options from the database.
    /// </summary>
    /// <param name="userKeyId">User key identifier.</param>
    /// <returns>Desktop model.</returns>
    Task<DesktopModel> GetWebUserDesktopOptionsAsync(int userKeyId);

    /// <summary>
    /// Get Web default user desktop options from the database.
    /// </summary>
    /// <returns>User desktop options.</returns>
    Task<DesktopModel> GetWebDefaultUserDesktopOptionsAsync();
}