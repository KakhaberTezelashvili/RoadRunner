using MasterDataService.Core.Repositories.Interfaces;
using MasterDataService.Core.Services.DesktopData;

namespace MasterDataService.Core.Test.Services.DesktopData;

public class DesktopDataServiceTests
{
    private const string _identifier = "1";
    private const string _data = "data";
    private const int _userKeyId = 1;
    private readonly IDesktopDataValidator _desktopDataValidator;

    // Service to test.
    private readonly DesktopDataService _desktopDataService;

    // Injected services.
    private readonly Mock<IDesktopDataRepository> _desktopDataRepository;

    public DesktopDataServiceTests()
    {
        _desktopDataRepository = new Mock<IDesktopDataRepository>();
        _desktopDataValidator = new DesktopDataValidator(_desktopDataRepository.Object);
        _desktopDataService = new DesktopDataService(_desktopDataRepository.Object, _desktopDataValidator);
    }

    #region GetComponentStateAsync

    [Fact]
    public async Task GetComponentStateAsync_ReturnsDesktopDataModel()
    {
        // Arrange
        _desktopDataRepository.Setup(r => r.GetByUserKeyIdAndDataIdentifierAsync(_userKeyId, _identifier)).ReturnsAsync(
            await Task.FromResult(new DesktopModel()));

        // Act
        DesktopModel desktopModel = await _desktopDataService.GetComponentStateAsync(_userKeyId, _identifier);

        // Assert
        Assert.NotNull(desktopModel);
    }

    [Fact]
    public async Task GetComponentStateAsync_IdentifierIsEmpty_ThrowsException()
    {
        // Arrange

        // Act
        Exception exception = await Record.ExceptionAsync(() => _desktopDataService.GetComponentStateAsync(_userKeyId, ""));

        // Assert
        Assert.NotNull(exception);
    }

    #endregion

    #region SetComponentStateAsync

    [Fact]
    public async Task SetComponentStateAsync_SetsSuccessfully()
    {
        // Arrange
        _desktopDataRepository.Setup(r => r.GetByUserKeyIdAndDataIdentifierAsync(_userKeyId, _identifier)).ReturnsAsync(
            await Task.FromResult(new DesktopModel()));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _desktopDataService.SetComponentStateAsync(_userKeyId, _identifier, _data));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task SetComponentStateAsync_DataIsEmpty_ThrowsException()
    {
        // Arrange
        _desktopDataRepository.Setup(r => r.GetByUserKeyIdAndDataIdentifierAsync(_userKeyId, _identifier)).ReturnsAsync(
            await Task.FromResult(new DesktopModel()));

        // Act
        var exception = await Record.ExceptionAsync(() => _desktopDataService.SetComponentStateAsync(_userKeyId, _identifier, string.Empty)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.Empty, exception.Code);
    }

    [Fact]
    public async Task SetComponentStateAsync_DataIsNull_CreatesNewEntity()
    {
        // Arrange
        _desktopDataRepository.Setup(r => r.GetByUserKeyIdAndDataIdentifierAsync(_userKeyId, _identifier)).ReturnsAsync(
            await Task.FromResult<DesktopModel>(null));

        // Act
        await _desktopDataService.SetComponentStateAsync(_userKeyId, _identifier, _data);

        // Assert
        _desktopDataRepository.Verify(r => r.AddAsync(It.IsAny<DesktopModel>()), Times.Once());
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(null, null)]
    public async Task SetComponentStateAsync_InvalidInput_ThrowsException(string identifier, string data)
    {
        // Arrange

        // Act
        Exception exception = await Record.ExceptionAsync(() => _desktopDataService.SetComponentStateAsync(_userKeyId, identifier, data));

        // Assert
        Assert.NotNull(exception);
    }

    #endregion
}