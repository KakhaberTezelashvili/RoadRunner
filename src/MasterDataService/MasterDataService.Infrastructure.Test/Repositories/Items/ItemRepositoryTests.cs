using MasterDataService.Infrastructure.Repositories.Items;
using MasterDataService.Infrastructure.Test.Repositories.Items;

namespace MasterDataService.Infrastructure.Test.Repositories;

public class ItemRepositoryTests : RepositoryBaseTests
{
    // Global constants.
    private readonly int _itemKeyId = 1;
    private readonly string _itemId = "MyItem";
    private readonly string _itemText = "My item";

    #region AddDataAsync

    [Fact]
    [Trait("Category", "ItemRepository.AddDataAsync")]
    public async Task AddDataAsync_ItemAddedSuccessfully()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var itemRepository = new ItemRepository(context);
        var newItem = new ItemModel() { Item = _itemId, Text = _itemText };

        // Act
        int itemKeyId = await itemRepository.AddDataAsync(newItem);
        context.Items.Remove(newItem);
        await context.SaveChangesAsync();

        // Assert
        Assert.True(itemKeyId > 0);
    }

    [Theory]
    [InlineData(null, "Text")]
    [InlineData("Item", null)]
    [Trait("Category", "ItemRepository.AddDataAsync")]
    public async Task AddDataAsync_NotDefinedRequiredField_ThrowsException(string item, string text)
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var itemRepository = new ItemRepository(context);
        var newItem = new ItemModel() { Item = item, Text = text };

        // Act
        Exception exception = await Record.ExceptionAsync(async () => await itemRepository.AddDataAsync(newItem));

        // Assert
        Assert.NotNull(exception);
    }

    #endregion AddDataAsync

    #region GetByKeyIdAsync

    [Fact]
    [Trait("Category", "ItemRepository.GetByKeyIdAsync")]
    public async Task GetByKeyIdAsync_EmptyTable_ReturnsNothing()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var itemRepository = new ItemRepository(context);

        // Act
        ItemModel? item = await itemRepository.GetByKeyIdAsync(_itemKeyId);

        // Assert
        Assert.Null(item);
    }

    [Fact]
    [Trait("Category", "ItemRepository.GetByKeyIdAsync")]
    public async Task GetByKeyIdAsync_ItemNotExist_ReturnsNothing()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        IList<ItemModel> mockItems = new List<ItemModel>
        {
            ItemRepositoryTestsData.CreateMockItemModel(_itemKeyId + 1)
        };
        await context.Items.AddRangeAsync(mockItems);
        await context.SaveChangesAsync();
        var itemRepository = new ItemRepository(context);

        // Act
        ItemModel? item = await itemRepository.GetByKeyIdAsync(_itemKeyId);
        context.Items.RemoveRange(mockItems);
        await context.SaveChangesAsync();

        // Assert
        Assert.Null(item);
    }

    [Fact]
    [Trait("Category", "ItemRepository.GetByKeyIdAsync")]
    public async Task GetByKeyIdAsync_ReturnsItem()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        IList<ItemModel> mockItems = new List<ItemModel>
        {
            ItemRepositoryTestsData.CreateMockItemModel(_itemKeyId),
            ItemRepositoryTestsData.CreateMockItemModel(_itemKeyId + 1)
        };
        await context.Items.AddRangeAsync(mockItems);
        await context.SaveChangesAsync();
        var itemRepository = new ItemRepository(context);

        // Act
        ItemModel? item = await itemRepository.GetByKeyIdAsync(_itemKeyId);
        context.Items.RemoveRange(mockItems);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(item);
        Assert.Equal(_itemKeyId, item.KeyId);
    }

    #endregion GetByKeyIdAsync

    #region GetByItemAsync

    [Fact]
    [Trait("Category", "ItemRepository.GetByItemAsync")]
    public async Task GetByItemAsync_ItemNotExist_ReturnsNothing()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        IList<ItemModel> mockItems = new List<ItemModel>
        {
            ItemRepositoryTestsData.CreateMockItemModel(_itemKeyId, _itemId + "2")
        };
        await context.Items.AddRangeAsync(mockItems);
        await context.SaveChangesAsync();
        var itemRepository = new ItemRepository(context);

        // Act
        ItemModel? item = await itemRepository.GetByItemAsync(_itemId);
        context.Items.RemoveRange(mockItems);
        await context.SaveChangesAsync();

        // Assert
        Assert.Null(item);
    }

    [Fact]
    [Trait("Category", "ItemRepository.GetByItemAsync")]
    public async Task GetByItemAsync_ReturnsItem()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        IList<ItemModel> mockItems = new List<ItemModel>
        {
            ItemRepositoryTestsData.CreateMockItemModel(_itemKeyId, _itemId),
            ItemRepositoryTestsData.CreateMockItemModel(_itemKeyId + 1, _itemId + "2")
        };
        await context.Items.AddRangeAsync(mockItems);
        await context.SaveChangesAsync();
        var itemRepository = new ItemRepository(context);

        // Act
        ItemModel? item = await itemRepository.GetByKeyIdAsync(_itemKeyId);
        context.Items.RemoveRange(mockItems);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(item);
        Assert.Equal(_itemKeyId, item.KeyId);
    }

    #endregion GetByItemAsync
}