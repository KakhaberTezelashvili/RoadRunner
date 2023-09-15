using TDOC.Common.Data.Enumerations.Errors;
using TDOC.Common.Data.Models.Exceptions;
using TDOC.Common.Server.Auth.Repositories;
using TDOC.Common.Server.Auth.Services;

namespace TDOC.Common.Server.Test.Auth.Services;

public class UserValidatorTests
{
    private const string _userInitials = "User1";

    // Service to test.
    private readonly IUserValidator _userValidator;

    // Injected services.
    private readonly Mock<IUserRepository> _userRepository;

    public UserValidatorTests()
    {
        _userRepository = new Mock<IUserRepository>();
        _userValidator = new UserValidator(_userRepository.Object);
    }

    #region AuthUserByInitialsValidate

    [Theory]
    [InlineData("  ")]
    [InlineData(" ")]
    [InlineData("")]
    [InlineData(null)]
    [Trait("Category", "UserValidator.AuthUserByInitialsValidate")]
    public void AuthUserByInitialsValidate_UserInitialsIsNullOrWhiteSpace_ThrowsException(string userInitials)
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => _userValidator.AuthUserByInitialsValidate(userInitials)) as InputArgumentException;

        //Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception?.Code);
    }

    [Fact]
    [Trait("Category", "UserValidator.AuthUserByInitialsValidate")]
    public void AuthUserByInitialsValidate_UserInitialsIsNotNullOrWhiteSpace_NotThrowsException()
    {
        // Arrange

        // Act
        Exception exception = Record.Exception(() => _userValidator.AuthUserByInitialsValidate(_userInitials));

        //Assert
        Assert.Null(exception);
    }

    #endregion AuthUserByInitialsValidate
}