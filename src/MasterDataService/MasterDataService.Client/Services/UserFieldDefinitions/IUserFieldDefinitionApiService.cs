namespace MasterDataService.Client.Services.UserFieldDefinitions;

/// <summary>
/// API service provides methods to retrieve/handle user field definition data.
/// </summary>
public interface IUserFieldDefinitionApiService
{
    /// <summary>
    /// Get user field definitions for specified table names.
    /// </summary>
    /// <param name="tableNames">List of table names.</param>
    /// <returns>Collection of user field definitions.</returns>
    Task<IList<UserFieldDefModel>> GetUserFieldDefinitionsAsync(IList<string> tableNames);
}