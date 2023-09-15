namespace MasterDataService.Core.Services.UserFieldDefinitions;

/// <inheritdoc cref="IUserFieldDefinitionService" />
public class UserFieldDefinitionService : IUserFieldDefinitionService
{
    private readonly IUserFieldDefinitionRepository _userFieldRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserFieldDefinitionService" /> class.
    /// </summary>
    /// <param name="userFieldRepository">Repository provides methods to retrieve/handle user field definitions.</param>
    public UserFieldDefinitionService(IUserFieldDefinitionRepository userFieldRepository)
    {
        _userFieldRepository = userFieldRepository;
    }

    /// <inheritdoc />
    public async Task<IList<UserFieldDefModel>> GetUserFieldDefinitionsAsync(List<string> tableNames) =>
        // TODO: Add validator
        await _userFieldRepository.GetUserFieldDefinitionsAsync(tableNames);
}