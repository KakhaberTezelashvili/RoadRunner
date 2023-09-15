using ProductionService.Infrastructure.Repositories;
using ProductionService.Shared.Dtos.Positions;

namespace ProductionService.Infrastructure.Test.Repositories;

public class PositionRepositoryTests : RepositoryBaseTests
{
    #region GetWithLocationsByKeyIdAsync

    [Theory]
    [InlineData(1000)]
    public async Task GetWithLocationsByKeyIdAsync_ReturnsNothing(int positionKeyId)
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var positionRepository = new PositionRepository(context);

        // Act
        PositionModel position = await positionRepository.GetWithLocationsByKeyIdAsync(positionKeyId);

        // Assert
        Assert.Null(position);
    }

    [Fact]
    public async Task GetWithLocationsByKeyIdAsync_ReturnsPositionLocations()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var positionModel = new PositionModel
        {
            KeyId = 1001,
            Name = "Position 1",
            PosPosLocationList = new List<PosLocationModel>
            {
                new PosLocationModel
                {
                    KeyId = 1001,
                    Loca = new LocationModel
                    {
                        KeyId = 1001,
                        Name = "Location 1",
                        Process = ProcessType.PackWF,
                        Status = 10
                    }
                },
                new PosLocationModel
                {
                    KeyId = 1002,
                    Loca = new LocationModel
                    {
                        KeyId = 1002,
                        Name = "Location 2",
                        Process = ProcessType.ReturnWF,
                        Status = 10
                    }
                }
            }
        };
        await context.Positions.AddAsync(positionModel);
        await context.SaveChangesAsync();
        var positionRepository = new PositionRepository(context);

        // Act
        PositionModel position = await positionRepository.GetWithLocationsByKeyIdAsync(positionModel.KeyId);
        context.Positions.Remove(positionModel);
        context.PositionLocations.RemoveRange(positionModel.PosPosLocationList);
        context.Locations.RemoveRange(positionModel.PosPosLocationList.Select(p => p.Loca));
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(position);
        Assert.Equal(positionModel.KeyId, position.KeyId);
        Assert.Equal(positionModel.PosPosLocationList.Count, position.PosPosLocationList.Count);
        Assert.Equal(positionModel.PosPosLocationList?[0].KeyId, position.PosPosLocationList?[0].KeyId);
    }

    #endregion GetWithLocationsByKeyIdAsync

    #region GetScannerLocationsByKeyIdAsync

    [Theory]
    [InlineData(1000)]
    public async Task GetScannerLocationsByKeyIdAsync_ReturnsNothing(int positionKeyId)
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var positionRepository = new PositionRepository(context);

        // Act
        IList<PositionLocationsDetailsDto> positions = await positionRepository.GetScannerLocationsByKeyIdAsync(positionKeyId);

        // Assert
        Assert.NotNull(positions);
        Assert.Empty(positions);
    }

    [Fact]
    public async Task GetScannerLocationsByKeyIdAsync_ReturnsScannerLocations()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var factoryModel = new FactoryModel
        {
            KeyId = 1001,
            Factory = "STC1001",
            Name = "Sterile Central Supply"
        };
        var positionModel = new PositionModel
        {
            KeyId = 1001,
            Name = "Position 1001",
            PosPosLocationList = new List<PosLocationModel>
            {
                new PosLocationModel
                {
                    KeyId = 1001,
                    Loca = new LocationModel
                    {
                        Fac = factoryModel,
                        KeyId = 1001,
                        Name = "Location 1001",
                        Process = ProcessType.PackWF,
                        Status = 10
                    }
                },
                new PosLocationModel
                {
                    KeyId = 1002,
                    Loca = new LocationModel
                    {
                        Fac = factoryModel,
                        KeyId = 1002,
                        Name = "Location 1002",
                        Process = ProcessType.ReturnWF,
                        Status = 10
                    }
                }
            }
        };
        await context.Positions.AddAsync(positionModel);
        await context.SaveChangesAsync();
        var positionRepository = new PositionRepository(context);

        // Act
        IList<PositionLocationsDetailsDto> positions = await positionRepository.GetScannerLocationsByKeyIdAsync(positionModel.KeyId);
        context.Positions.Remove(positionModel);
        context.Factories.Remove(factoryModel);
        context.PositionLocations.RemoveRange(positionModel.PosPosLocationList);
        context.Locations.RemoveRange(positionModel.PosPosLocationList.Select(p => p.Loca));
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(positions);
        Assert.Equal(positionModel.PosPosLocationList.Count, positions.Count);
        Assert.Equal(positionModel.PosPosLocationList[0].Loca.KeyId, positions[1].LocationKeyId);
        Assert.Equal(positionModel.PosPosLocationList[1].Loca.KeyId, positions[0].LocationKeyId);
    }

    #endregion GetScannerLocationsByKeyIdAsync
}