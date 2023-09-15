using MasterDataService.Core.Repositories.Interfaces;
using MasterDataService.Core.Services.UserFieldDefinitions;

namespace MasterDataService.Core.Test.Services.UserFieldDefinition;

public class UserFieldDefinitionServiceTests
{
    private const string _tableName = "";
    private const int _autoInc = 1;

    // Service to test.
    private readonly UserFieldDefinitionService _userFieldDefinitionService;

    // Injected services.
    private readonly Mock<IUserFieldDefinitionRepository> _userFieldDefinitionRepository;

    public UserFieldDefinitionServiceTests()
    {
        _userFieldDefinitionRepository = new Mock<IUserFieldDefinitionRepository>();
        _userFieldDefinitionService = new UserFieldDefinitionService(_userFieldDefinitionRepository.Object);
    }

    #region GetUserFieldDefinitionsAsync

    [Fact]
    public async Task GetUserFieldDefinitionsAsync_ReturnsNothing()
    {
        // Arrange
        var mockTableNames = new List<string>() { _tableName };

        _userFieldDefinitionRepository.Setup(r => r.GetUserFieldDefinitionsAsync(mockTableNames)).ReturnsAsync(
            await Task.FromResult<IList<UserFieldDefModel>>(null));

        // Act
        IList<UserFieldDefModel> userFieldDefModels = await _userFieldDefinitionService.GetUserFieldDefinitionsAsync(mockTableNames);

        // Assert
        Assert.Null(userFieldDefModels);
    }

    [Fact]
    public async Task GetUserFieldDefinitionsAsync_ReturnsUserFieldDefModels()
    {
        // Arrange
        var mockTableNames = new List<string>() { _tableName };
        var mockUserFieldDefModels = new List<UserFieldDefModel>
        {
            new UserFieldDefModel { AutoInc = _autoInc },
            new UserFieldDefModel { AutoInc = _autoInc }
        };

        _userFieldDefinitionRepository.Setup(r => r.GetUserFieldDefinitionsAsync(mockTableNames)).ReturnsAsync(
            await Task.FromResult(mockUserFieldDefModels));

        // Act
        IList<UserFieldDefModel> userFieldDefModels = await _userFieldDefinitionService.GetUserFieldDefinitionsAsync(mockTableNames);

        // Assert
        Assert.NotNull(userFieldDefModels);
        Assert.Equal(mockUserFieldDefModels.Count, userFieldDefModels.Count);
        Assert.Equal(mockUserFieldDefModels[0].AutoInc, userFieldDefModels[0].AutoInc);
        Assert.Equal(mockUserFieldDefModels[1].AutoInc, userFieldDefModels[1].AutoInc);
    }

    #endregion
}