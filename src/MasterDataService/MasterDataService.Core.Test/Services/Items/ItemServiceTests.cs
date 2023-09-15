using MasterDataService.Core.Repositories.Interfaces.Items;
using MasterDataService.Core.Services.Items;
using MasterDataService.Shared.Dtos.Items;

namespace MasterDataService.Core.Test.Services.Items;

public class ItemServiceTests
{
    private const int _userKeyId = 1;
    private const int _itemKeyId = 1;
    private const string _itemItem = "MyItem";
    private const string _itemText = "My item";

    // Service to test.
    private readonly ItemService _itemService;

    // Injected services.
    private readonly Mock<IItemRepository> _itemRepository;
    private readonly Mock<IItemValidator> _itemValidator;

    public ItemServiceTests()
    {
        _itemRepository = new Mock<IItemRepository>();
        _itemValidator = new Mock<IItemValidator>();
        _itemService = new ItemService(_itemValidator.Object, _itemRepository.Object);
    }

    #region GetByKeyIdAsync

    [Fact]
    [Trait("Category", "ItemService.GetByKeyIdAsync")]
    public async Task GetByKeyIdAsync_ItemNotExist_ThrowsException()
    {
        // Arrange
        _itemValidator.Setup(v => v.FindByKeyIdValidateAsync(_itemKeyId, _itemRepository.Object.GetByKeyIdAsync, 1))
            .ThrowsAsync(new Exception());

        // Act
        Exception exception = await Record.ExceptionAsync(() => _itemService.GetByKeyIdAsync(_itemKeyId));

        // Assert
        Assert.NotNull(exception);
    }

    [Fact]
    [Trait("Category", "ItemService.GetByKeyIdAsync")]
    public async Task GetByKeyIdAsync_ReturnsItemData()
    {
        // Arrange
        _itemValidator.Setup(v => v.FindByKeyIdValidateAsync(_itemKeyId, _itemRepository.Object.GetByKeyIdAsync, 1))
            .ReturnsAsync(await Task.FromResult(new ItemModel()));

        // Act
        ItemModel item = await _itemService.GetByKeyIdAsync(_itemKeyId);

        // Assert
        Assert.NotNull(item);
    }

    #endregion GetByKeyIdAsync

    #region AddDataAsync

    [Fact]
    [Trait("Category", "ItemService.AddDataAsync")]
    public async Task AddDataAsync_ValidationFails_ThrowsException()
    {
        // Arrange
        var args = new ItemAddArgs() { Item = _itemItem, Text = _itemText, UserKeyId = _userKeyId };
        _itemValidator.Setup(v => v.AddDataValidateAsync(args)).ThrowsAsync(new Exception());

        // Act
        Exception exception = await Record.ExceptionAsync(() => _itemService.AddDataAsync(args));

        // Assert
        Assert.NotNull(exception);
    }

    [Fact]
    [Trait("Category", "ItemService.AddDataAsync")]
    public async Task AddDataAsync_ReturnsNewImetKeyId()
    {
        // Arrange
        var args = new ItemAddArgs() { Item = _itemItem, Text = _itemText, UserKeyId = _userKeyId };
        _itemRepository.Setup(r => r.AddDataAsync(It.IsAny<ItemModel>())).ReturnsAsync(await Task.FromResult(_itemKeyId));

        // Act
        int newItemKeyId = await _itemService.AddDataAsync(args);

        // Assert
        Assert.True(newItemKeyId == _itemKeyId);
    }

    #endregion AddDataAsync

    #region UpdateDataAsync

    [Fact]
    [Trait("Category", "ItemService.UpdateDataAsync")]
    public async Task UpdateDataAsync_ValidationFails_ThrowsException()
    {
        // Arrange
        var args = new ItemUpdateArgs();
        _itemValidator.Setup(v => v.UpdateDataValidateAsync(args)).ThrowsAsync(new Exception());

        // Act
        Exception exception = await Record.ExceptionAsync(() => _itemService.UpdateDataAsync(args));

        // Assert
        Assert.NotNull(exception);
        _itemRepository.Verify(r => r.UpdateAsync(It.IsAny<ItemModel>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "ItemService.UpdateDataAsync")]
    public async Task UpdateDataAsync_UpdatesItemSuccessfully()
    {
        // Arrange
        var args = new ItemUpdateArgs() { KeyId = _itemKeyId, Item = _itemItem, Text = _itemText, UserKeyId = _userKeyId };
        _itemValidator.Setup(v => v.UpdateDataValidateAsync(args)).ReturnsAsync(await Task.FromResult(new ItemModel() { KeyId = args.KeyId }));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _itemService.UpdateDataAsync(args));

        // Assert
        Assert.Null(exception);
        _itemRepository.Verify(r => r.UpdateAsync(It.IsAny<ItemModel>()), Times.Once);
    }

    #endregion UpdateDataAsync
}