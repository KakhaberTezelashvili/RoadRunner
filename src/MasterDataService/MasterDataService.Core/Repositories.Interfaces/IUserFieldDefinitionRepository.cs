namespace MasterDataService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle user field definitions.
/// </summary>
public interface IUserFieldDefinitionRepository
{
    /// <summary>
    /// Get user field definitions for specified table names.
    /// </summary>
    /// <param name="tableNames">List of table names.</param>
    /// <returns>List of user field definitions.</returns>
    Task<IList<UserFieldDefModel>> GetUserFieldDefinitionsAsync(List<string> tableNames);
}