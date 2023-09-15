using ProductionService.Infrastructure.Repositories;

namespace ProductionService.Infrastructure.Test.Repositories;

public class SerialRepositoryTests : RepositoryBaseTests
{
    #region GetByKeyIdAsync

    [Theory]
    [InlineData(1000)]
    public async Task GetByKeyIdAsync_ReturnsNothing(int serialKeyId)
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var serialRepository = new SerialRepository(context);

        // Act
        SerialModel serial = await serialRepository.GetByKeyIdAsync(serialKeyId);

        // Assert
        Assert.Null(serial);
    }

    [Theory]
    [InlineData(1066, "S-01101-001", 1001)]
    [InlineData(1001, "01101-001", 1001)]
    public async Task GetSerialAsync_ReturnsSerial(int serialKeyId, string serialNo, int? refProdKeyId)
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var serialModel = new SerialModel
        {
            KeyId = serialKeyId,
            SerialNo = serialNo,
            RefProdKeyId = refProdKeyId
        };
        await context.SerialNumbers.AddAsync(serialModel);
        await context.SaveChangesAsync();
        var serialRepository = new SerialRepository(context);

        // Act
        SerialModel serial = await serialRepository.GetByKeyIdAsync(serialKeyId);
        context.SerialNumbers.Remove(serialModel);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(serial);
        Assert.Equal(serialKeyId, serial.KeyId);
        Assert.Equal(serialNo, serial.SerialNo);
        Assert.Equal(refProdKeyId, serial.RefProdKeyId);
    }

    #endregion GetByKeyIdAsync
}