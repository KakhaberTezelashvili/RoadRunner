using MasterDataService.Core.Repositories.Interfaces.Items.Recent;
using MasterDataService.Core.Services.Items.Recent;

namespace MasterDataService.Core.Test.Services.Items;

public class ItemRecentServiceTests
{
    // Service to test.
    private readonly ItemRecentService _itemRecentService;

    // Injected services.
    private readonly Mock<IItemRecentRepository> _itemRepository;

    public ItemRecentServiceTests()
    {
        _itemRepository = new Mock<IItemRecentRepository>();
        _itemRecentService = new ItemRecentService(_itemRepository.Object);
    }

    #region GetRecentAsync

    [Fact]
    [Trait("Category", "ItemRecentService.GetRecentAsync")]
    public async Task GetRecentAsync_ReturnsNothing()
    {
        // Arrange

        // Act
        ItemModel? item = await _itemRecentService.GetRecentAsync();

        // Assert
        Assert.Null(item);
    }

    [Fact]
    [Trait("Category", "ItemRecentService.GetRecentAsync")]
    public async Task GetRecentAsync_ReturnsItem()
    {
        // Arrange
        _itemRepository.Setup(r => r.GetLastAsync()).ReturnsAsync(await Task.FromResult(new ItemModel()));

        // Act
        ItemModel? item = await _itemRecentService.GetRecentAsync();

        // Assert
        Assert.NotNull(item);
    }

    #endregion GetRecentAsync
}