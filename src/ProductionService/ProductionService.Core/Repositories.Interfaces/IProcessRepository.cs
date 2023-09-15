using ProductionService.Shared.Dtos.Processes;

namespace ProductionService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle processes.
/// </summary>
public interface IProcessRepository : IRepositoryBase<ProcessModel>
{
    /// <summary>
    /// Get list of processes by machine.
    /// </summary>
    /// <param name="machineKeyId">Machine key identifier.</param>
    /// <returns>List of processes.</returns>
    Task<IList<ProcessModel>> GetProcessesByMachineAsync(
        int machineKeyId, ProcessStatus? status = null, bool orderByDesc = false);

    /// <summary>
    /// Create a new process.
    /// </summary>
    /// <param name="data">ProcessModel data for a new process.</param>
    /// <returns>Key id of just created process.</returns>
    Task<int> CreateProcessAsync(ProcessModel data);

    /// <summary>
    /// Get process by batch no.
    /// </summary>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <returns></returns>
    Task<ProcessModel> GetProcessAsync(int batchKeyId);

    /// <summary>
    /// Updates process.
    /// </summary>
    /// <param name="data">ProcessModel data.</param>
    /// <returns></returns>
    Task UpdateProcessAsync(ProcessModel data);

    /// <summary>
    /// Retrieves details of batch by a batch key id.
    /// </summary>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <returns>BatchDetailsDto indicating the result of the operation.</returns>
    Task<BatchDetailsDto> GetBatchDetailsByBatchKeyIdAsync(int batchKeyId);
}