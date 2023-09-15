using Microsoft.EntityFrameworkCore;
using ProductionService.Infrastructure.Repositories;

namespace ProductionService.Infrastructure.Test.Repositories;

public class ProcessRepositoryTests : RepositoryBaseTests
{
    private const int _batchNo = 1001;

    #region CreateProcessAsync

    [Fact]
    public async Task CreateProcessAsync_SaveDataAndReturnKeyId()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var processRepository = new ProcessRepository(context);
        ProcessModel mockData = new();

        // Act
        int response = await processRepository.CreateProcessAsync(mockData);
        context.Remove(mockData);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(mockData.KeyId, response);
    }

    [Fact]
    public async Task CreateProcessAsync_ExternalBatchNoLongerThan25Characters()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var processRepository = new ProcessRepository(context);
        ProcessModel mockData = new()
        {
            KeyId = 1001,
            ExternalBatchNo = "This string is longer than 25 characters"
        };

        // Act
        Exception error = await Record.ExceptionAsync(async () => await processRepository.CreateProcessAsync(mockData));
        context.Remove(mockData);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(error);
    }

    #endregion CreateProcessAsync

    #region GetProcessesByMachineAsync

    [Fact]
    public async Task GetProcessesByMachineAsync_ReturnsProcessesWithMachineKeyIdParameter()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        int mockMachineKeyId = 1001;
        var mockProcessModels = new List<ProcessModel>
        {
            new ProcessModel
            {
                KeyId = 1001,
                Status = ProcessStatus.Done,
                MachKeyId = mockMachineKeyId
            },
            new ProcessModel
            {
                KeyId = 1002,
                Status = ProcessStatus.Initiated,
                MachKeyId = 1002
            },
            new ProcessModel
            {
                KeyId = 1003,
                Status = ProcessStatus.Running,
                MachKeyId = mockMachineKeyId
            },
        };
        await context.Processes.AddRangeAsync(mockProcessModels);
        await context.SaveChangesAsync();
        var processRepository = new ProcessRepository(context);

        // Act
        IList<ProcessModel> response = await processRepository.GetProcessesByMachineAsync(mockMachineKeyId);
        context.RemoveRange(mockProcessModels);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Count);
        Assert.Equal(mockProcessModels[0].KeyId, response[0].KeyId);
        Assert.Equal(mockProcessModels[2].KeyId, response[1].KeyId);
    }

    [Fact]
    public async Task GetProcessesByMachineAsync_StatusIsNotNull()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        int mockMachineKeyId = 1001;
        ProcessStatus mockProcessStatus = ProcessStatus.Running;
        var mockProcessModels = new List<ProcessModel>
        {
            new ProcessModel
            {
                KeyId = 1001,
                Status = ProcessStatus.Ending,
                MachKeyId = mockMachineKeyId
            },
            new ProcessModel
            {
                KeyId = 1002,
                Status = ProcessStatus.Initiated,
                MachKeyId = mockMachineKeyId
            },
            new ProcessModel
            {
                KeyId = 1003,
                Status = ProcessStatus.Running,
                MachKeyId = mockMachineKeyId
            },
        };
        await context.Processes.AddRangeAsync(mockProcessModels);
        await context.SaveChangesAsync();
        var processRepository = new ProcessRepository(context);

        // Act
        IList<ProcessModel> response = await processRepository.GetProcessesByMachineAsync(mockMachineKeyId, mockProcessStatus);
        context.RemoveRange(mockProcessModels);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(response);
        Assert.Equal(1, response.Count);
        Assert.Equal(mockProcessStatus, response[0].Status);
        Assert.Equal(mockMachineKeyId, response[0].MachKeyId);
        Assert.Equal(mockProcessModels[2].KeyId, response[0].KeyId);
    }

    [Fact]
    public async Task GetProcessesByMachineAsync_OrderedByDescendingKeyId()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        int mockMachineKeyId = 1001;
        var mockProcessModels = new List<ProcessModel>
        {
            new ProcessModel
            {
                KeyId = 1001,
                Status = ProcessStatus.Done,
                MachKeyId = mockMachineKeyId
            },
            new ProcessModel
            {
                KeyId = 1002,
                Status = ProcessStatus.Initiated,
                MachKeyId = mockMachineKeyId
            },
            new ProcessModel
            {
                KeyId = 1003,
                Status = ProcessStatus.Running,
                MachKeyId = mockMachineKeyId
            },
        };
        mockProcessModels = mockProcessModels.OrderBy(p => p.KeyId).ToList();
        await context.Processes.AddRangeAsync(mockProcessModels);
        await context.SaveChangesAsync();
        var processRepository = new ProcessRepository(context);

        // Act
        IList<ProcessModel> response = await processRepository.GetProcessesByMachineAsync(mockMachineKeyId, null, true);
        context.RemoveRange(mockProcessModels);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(response);
        Assert.Equal(mockProcessModels.Count, response.Count);
        Assert.Equal(mockProcessModels[2].KeyId, response[0].KeyId);
        Assert.Equal(mockProcessModels[1].KeyId, response[1].KeyId);
        Assert.Equal(mockProcessModels[0].KeyId, response[2].KeyId);
    }

    #endregion GetProcessesByMachineAsync

    #region GetProcessAsync

    [Fact]
    public async Task GetProcessAsync_ReturnsNothing()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var processRepository = new ProcessRepository(context);

        // Act
        ProcessModel batch = await processRepository.GetProcessAsync(_batchNo);

        // Assert
        Assert.Null(batch);
    }

    [Fact]
    public async Task GetProcessAsync_ReturnsProcess()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var processRepository = new ProcessRepository(context);
        var process = new ProcessModel { KeyId = _batchNo };
        await context.Processes.AddAsync(process);
        await context.SaveChangesAsync();

        // Act
        ProcessModel batch = await processRepository.GetProcessAsync(_batchNo);
        context.Remove(process);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(batch);
        Assert.Equal(process.KeyId, batch.KeyId);
    }

    #endregion GetProcessAsync

    #region UpdateProcessAsync

    [Fact]
    public async Task UpdateProcessAsync_ProcessNotAvailable_ThrowsException()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var processRepository = new ProcessRepository(context);
        var updateProcess = new ProcessModel
        {
            KeyId = _batchNo,
            Status = ProcessStatus.Done
        };

        // Act
        Exception exception = await Record.ExceptionAsync(() => processRepository.UpdateProcessAsync(updateProcess));

        // Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task UpdateProcessAsync_UpdatesProcess()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var processRepository = new ProcessRepository(context);
        var process = new ProcessModel
        {
            KeyId = _batchNo,
            Status = ProcessStatus.Running
        };
        var updateProcess = new ProcessModel
        {
            KeyId = _batchNo,
            Status = ProcessStatus.Done
        };
        await context.Processes.AddAsync(process);
        await context.SaveChangesAsync();
        context.Entry(process).State = EntityState.Detached;

        // Act
        await processRepository.UpdateProcessAsync(updateProcess);
        ProcessModel updatedProcess = await context.Processes.FirstOrDefaultAsync(p => p.KeyId == _batchNo);
        context.Remove(updatedProcess);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(updateProcess.Status, updatedProcess.Status);
    }

    #endregion UpdateProcessAsync
}