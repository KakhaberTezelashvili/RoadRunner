namespace MasterDataService.Core.Services.UserFieldDefinitions;

/// <summary>
/// Service provides methods to retrieve/handle user field definitions.
/// </summary>
public interface IUserFieldDefinitionService
{
    /// <summary>
    /// Get user field definitions for specified table names.
    /// </summary>
    /// <param name="tableNames">List of table names.</param>
    /// <returns>List of user field definitions.</returns>
    Task<IList<UserFieldDefModel>> GetUserFieldDefinitionsAsync(List<string> tableNames);
}