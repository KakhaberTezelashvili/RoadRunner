namespace MasterDataService.Infrastructure.Repositories;

/// <inheritdoc cref="IUserFieldDefinitionRepository" />
public class UserFieldDefinitionRepository : RepositoryBase<UserFieldDefModel>, IUserFieldDefinitionRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserFieldDefinitionRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public UserFieldDefinitionRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IList<UserFieldDefModel>> GetUserFieldDefinitionsAsync(List<string> tableNames)
    {
        return await _context.UserFieldDefinitions.AsNoTracking()
            .Where(ufd => ufd.Active && tableNames.Contains(ufd.TableName.ToUpper()))
            .ToListAsync();
    }
}