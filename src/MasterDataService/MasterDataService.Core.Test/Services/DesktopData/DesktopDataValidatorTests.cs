using MasterDataService.Core.Repositories.Interfaces;
using MasterDataService.Core.Services.DesktopData;

namespace MasterDataService.Core.Test.Services.DesktopData;

public class DesktopDataValidatorTests
{
    private const string _identifier = "1";
    private const string _data = "data";

    // Service to test.
    private readonly IDesktopDataValidator _desktopDataValidator;

    // Injected services.
    private readonly Mock<IDesktopDataRepository> _desktopDataRepository;

    public DesktopDataValidatorTests()
    {
        _desktopDataRepository = new Mock<IDesktopDataRepository>();
        _desktopDataValidator = new DesktopDataValidator(_desktopDataRepository.Object);
    }

    #region GetComponentStateValidate

    [Fact]
    [Trait("Category", "DesktopDataValidator.GetComponentStateValidate")]
    public void GetComponentStateValidate_IdentifierIsEmpty_ThrowsException()
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => _desktopDataValidator.GetComponentStateValidate(string.Empty)) as InputArgumentException;

        //Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.Empty, exception.Code);
    }

    [Fact]
    [Trait("Category", "DesktopDataValidator.GetComponentStateValidate")]
    public void GetComponentStateValidate_IdentifierIsNotEmpty_NotThrowsException()
    {
        // Arrange
        
        // Act
        Exception exception = Record.Exception(() => _desktopDataValidator.GetComponentStateValidate(_identifier));

        //Assert
        Assert.Null(exception);
    }

    #endregion GetComponentStateValidate

    #region SetComponentStateValidate

    [Fact]
    [Trait("Category", "DesktopDataValidator.SetComponentStateValidate")]
    public void SetComponentStateValidate_IdentifierIsEmpty_ThrowsException()
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => _desktopDataValidator.SetComponentStateValidate("", _data)) as InputArgumentException;

        //Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.Empty, exception.Code);
    }

    [Fact]
    [Trait("Category", "DesktopDataValidator.SetComponentStateValidate")]
    public void SetComponentStateValidate_DataIsEmpty_ThrowsException()
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => _desktopDataValidator.SetComponentStateValidate(_identifier, "")) as InputArgumentException;

        //Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.Empty, exception.Code);
    }

    [Fact]
    [Trait("Category", "DesktopDataValidator.SetComponentStateValidate")]
    public void SetComponentStateValidate_IdentifierIsNotEmpty_NotThrowsException()
    {
        // Arrange
        
        // Act
        Exception exception = Record.Exception(() => _desktopDataValidator.SetComponentStateValidate(_identifier, _data));

        //Assert
        Assert.Null(exception);
    }

    #endregion SetComponentStateValidate
}
