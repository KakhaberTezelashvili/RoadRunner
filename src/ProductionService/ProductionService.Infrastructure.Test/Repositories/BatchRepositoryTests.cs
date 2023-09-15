using Bogus;
using FluentAssertions;
using ProductionService.Infrastructure.Repositories;
using Xunit.Extensions.AssertExtensions;

namespace ProductionService.Infrastructure.Test.Repositories;

public class BatchRepositoryTests : RepositoryBaseTests
{
    private const int _batchNo = 2001;

    #region AddBatchAsync

    [Fact]
    public async Task AddBatchAsync_BatchIsZero_ThrowsException()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var batchRepository = new BatchRepository(context);
        var batchModel = new BatchModel
        {
            Batch = 0,
            Unit = 1001,
            Type = (int)BatchType.PrimarySteri
        };

        // Act
        Exception error = await Record.ExceptionAsync(async () => await batchRepository.AddAsync(batchModel));
        context.Remove(batchModel);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(error);
    }

    [Fact]
    public async Task AddBatchAsync_UnitIsZero_ThrowsException()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var batchRepository = new BatchRepository(context);
        var batchModel = new BatchModel
        {
            Batch = _batchNo,
            Unit = 0,
            Type = (int)BatchType.PrimarySteri
        };

        // Act
        Exception error = await Record.ExceptionAsync(async () => await batchRepository.AddAsync(batchModel));
        context.Remove(batchModel);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(error);
    }

    [Fact]
    public async Task AddBatchAsync_BatchIsNotZeroAndUnitIsNotZero_NotThrowsException()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var batchRepository = new BatchRepository(context);
        var batchModel = new BatchModel
        {
            Batch = _batchNo,
            Unit = 1002,
            Type = (int)BatchType.PrimarySteri,
            ChPlKeyId = 1002,
            Status = BatchUnitStatus.OK
        };

        // Act
        Exception error = await Record.ExceptionAsync(async () => await batchRepository.AddAsync(batchModel));
        var batches = context.Batches.ToList();
        context.Remove(batchModel);
        await context.SaveChangesAsync();

        // Assert
        Assert.Null(error);
        Assert.Single(batches);
        Assert.Equal(batchModel.Batch, batches[0].Batch);
        Assert.Equal(batchModel.Unit, batches[0].Unit);
        Assert.Equal(batchModel.Type, batches[0].Type);
        Assert.Equal(batchModel.ChPlKeyId, batches[0].ChPlKeyId);
        Assert.Equal(batchModel.Status, batches[0].Status);
    }

    #endregion AddBatchAsync

    #region FindUnFailedBatchesByBatchKeyIdAsync

    [Fact]
    public async Task FindUnFailedBatchesByBatchKeyIdAsync_NotThrowsException()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var batchRepository = new BatchRepository(context);
        int batchKeyId = 905;
        List<BatchModel> batches = new()
        {
            new()
            {
                Batch = batchKeyId,
                Unit = 1002,
                Type = (int)BatchType.PrimarySteri,
                ChPlKeyId = 1002,
                Status = BatchUnitStatus.OK
            },
            new()
            {
                Batch = batchKeyId,
                Unit = 1003,
                Type = (int)BatchType.PrimarySteri,
                ChPlKeyId = 1002,
                Status = BatchUnitStatus.BatchFailed
            }
        };
        context.Batches.AddRange(batches);
        await context.SaveChangesAsync();

        // Act
        var unfailedBatches = (await batchRepository.FindUnFailedBatchesByBatchKeyIdAsync(batchKeyId)).ToList();
        context.Batches.RemoveRange(batches);
        await context.SaveChangesAsync();

        // Assert
        unfailedBatches.Should().HaveCount(2);
        unfailedBatches.Select(c => c.Batch).Should().AllBeEquivalentTo(batchKeyId);
        unfailedBatches.Select(c => c.Status).ShouldNotContain(BatchUnitStatus.UnitFailed);
    }

    #endregion FindUnFailedBatchesByBatchKeyIdAsync

    #region FindByUnitKeyIdAndBatchTypeAsync

    [Fact]
    public async Task FindByUnitKeyIdAndBatchTypeAsync_NotThrowsException()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var batchRepository = new BatchRepository(context);
        int UnitKeyId = 10001;
        List<BatchModel> batches = new()
        {
            new()
            {
                Batch = 1,
                Unit = UnitKeyId,
                Type = (int)BatchType.PrimarySteri,
                ChPlKeyId = 1002,
                Status = BatchUnitStatus.OK
            },
            new()
            {
                Batch = 2,
                Unit = UnitKeyId,
                Type = (int)BatchType.PrimarySteri,
                ChPlKeyId = 1002,
                Status = BatchUnitStatus.BatchFailed
            }
        };
        context.Batches.AddRange(batches);
        await context.SaveChangesAsync();

        // Act
        var dbBatches = (await batchRepository.FindByUnitKeyIdAndBatchTypeAsync(UnitKeyId, BatchType.PrimarySteri)).ToList();
        context.Batches.RemoveRange(dbBatches);
        await context.SaveChangesAsync();

        // Assert
        dbBatches.Should().HaveCount(2);
        dbBatches.Select(c => c.Type).Should().AllBeEquivalentTo((int)BatchType.PrimarySteri);
    }

    #endregion FindByUnitKeyIdAndBatchTypeAsync

    #region FindByBatchKeyIdAsync

    [Fact]
    public async Task FindByBatchKeyIdAsync_NotThrowsException()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var batchRepository = new BatchRepository(context);

        int batchKeyId = 11101;
        List<BatchModel> batches = new()
        {
            new()
            {
                Batch = batchKeyId,
                Unit = 1,
                Type = (int)BatchType.PrimarySteri,
                ChPlKeyId = 1002,
                Status = BatchUnitStatus.OK
            },
            new()
            {
                Batch = batchKeyId,
                Unit = 2,
                Type = (int)BatchType.PrimarySteri,
                ChPlKeyId = 1002,
                Status = BatchUnitStatus.BatchFailed
            }
        };
        context.Batches.AddRange(batches);
        await context.SaveChangesAsync();

        // Act
        var dbBatches = (await batchRepository.FindByBatchKeyIdAsync(batchKeyId)).ToList();
        context.Batches.RemoveRange(dbBatches);
        await context.SaveChangesAsync();

        // Assert
        dbBatches.Should().HaveCount(2);
        dbBatches.Select(c => c.Batch).Should().AllBeEquivalentTo(batchKeyId);
    }

    #endregion FindByBatchKeyIdAsync

    #region GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync

    [Fact]
    public async Task GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync_ReturnsCorrectData()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var batchRepository = new BatchRepository(context);

        var unit1 = new UnitModel { KeyId = 100011 };
        var unit2 = new UnitModel { KeyId = 100022 };

        int id = 10001;
        Faker<BatchModel> testBatches = new Faker<BatchModel>()
            .RuleFor(o => o.Batch, _ => id++)
            .RuleFor(u => u.UnitUnit, (f, b) => b.Batch <= 10003 ? unit1 : unit2)
            .RuleFor(u => u.Type, _ => (int)BatchType.PrimarySteri)
            .RuleFor(u => u.Status, _ => BatchUnitStatus.OK)
            .RuleFor(u => u.ChPlKeyId, _ => 100);

        List<BatchModel> batches = testBatches.Generate(6);

        // Improves insert speed
        context.ChangeTracker.AutoDetectChangesEnabled = false;
        context.Batches.AddRange(batches);
        await context.SaveChangesAsync();

        // Act
        var dbBatches = (await batchRepository.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(batches.Select(b => b.Unit).ToList(), BatchType.PrimarySteri)).ToList();

        context.Batches.RemoveRange(batches);
        context.Units.RemoveRange(batches.Select(b => b.UnitUnit));
        await context.SaveChangesAsync();

        // Assert
        dbBatches.Should().HaveCount(2);
        dbBatches.Select(b => b.BatchKeyId).Should().Equal(new List<int?> { 10003, 10006 });
    }

    #endregion GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync
}