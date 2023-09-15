using TDOC.Common.Server.Auth.Models;
using TDOC.Common.Server.Auth.Repositories;
using TDOC.Common.Server.Auth.Services;

namespace TDOC.Common.Server.Test.Auth.Services;

public class UserServiceTests
{
    private const string _userInitials = "User1";

    private readonly IUserValidator _userValidator;

    // Service to test.
    private readonly UserService _userService;

    // Injected services.
    private readonly Mock<IUserRepository> _userRepository;

    public UserServiceTests()
    {
        _userRepository = new Mock<IUserRepository>();
        _userValidator = new UserValidator(_userRepository.Object);
        _userService = new UserService(_userRepository.Object, _userValidator);
    }

    #region GetAuthUserByInitialsAsync

    [Fact]
    public async Task GetAuthUserByInitialsAsync_ReturnsFailedValidateBeforeAuthUserData()
    {
        // Arrange

        // Act
        Exception exception = await Record.ExceptionAsync(() => _userService.GetAuthUserByInitialsAsync(null));

        //Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task GetAuthUserByInitialsAsync_ReturnsNothing()
    {
        // Arrange
        _userRepository.Setup(r => r.GetAuthUserByInitialsAsync(_userInitials)).ReturnsAsync(
            await Task.FromResult<AuthUserData>(null));

        // Act
        AuthUserData userData = await _userService.GetAuthUserByInitialsAsync(_userInitials);

        // Assert
        Assert.Null(userData);
    }

    [Fact]
    public async Task GetAuthUserByInitialsAsync_ReturnsAuthUserData()
    {
        // Arrange
        _userRepository.Setup(r => r.GetAuthUserByInitialsAsync(_userInitials)).ReturnsAsync(
            await Task.FromResult(new AuthUserData()));

        // Act
        AuthUserData userData = await _userService.GetAuthUserByInitialsAsync(_userInitials);

        // Assert
        Assert.NotNull(userData);
    }

    #endregion
}