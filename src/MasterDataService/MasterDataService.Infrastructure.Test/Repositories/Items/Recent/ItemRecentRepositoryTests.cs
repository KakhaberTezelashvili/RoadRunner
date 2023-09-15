using MasterDataService.Infrastructure.Repositories.Items.Recent;

namespace MasterDataService.Infrastructure.Test.Repositories.Items.Recent;

public class ItemRecentRepositoryTests : RepositoryBaseTests
{
    private readonly int _itemKeyId = 1;

    #region GetLastAsync

    [Fact]
    [Trait("Category", "ItemRecentRepository.GetLastAsync")]
    public async Task GetLastAsync_ReturnsNothing()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var itemRecentRepository = new ItemRecentRepository(context);

        // Act
        ItemModel? item = await itemRecentRepository.GetLastAsync();

        // Assert
        Assert.Null(item);
    }

    [Fact]
    [Trait("Category", "ItemRecentRepository.GetLastAsync")]
    public async Task GetLastAsync_ReturnsLastItem()
    {
        // Arrange
        int lastItemKeyId = 2;
        await using TDocEFDbContext context = ConfigureContext();
        IList<ItemModel> mockItems = new List<ItemModel>
        {
            ItemRepositoryTestsData.CreateMockItemModel(_itemKeyId),
            ItemRepositoryTestsData.CreateMockItemModel(lastItemKeyId)
        };

        await context.Items.AddRangeAsync(mockItems);
        await context.SaveChangesAsync();

        var itemRecentRepository = new ItemRecentRepository(context);

        // Act
        ItemModel? item = await itemRecentRepository.GetLastAsync();

        context.Items.RemoveRange(mockItems);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(item);
        Assert.Equal(lastItemKeyId, item.KeyId);
    }

    #endregion GetLastAsync
}