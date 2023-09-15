using ProductionService.Shared.Dtos.Processes;

namespace ProductionService.Core.Services.Processes;

/// <summary>
/// Service provides methods to retrieve/handle processes.
/// </summary>
public interface IProcessService
{
    /// <summary>
    /// Create a new batch.
    /// </summary>
    /// <param name="args">Input arguments used for creating new batch.</param>
    /// <param name="userKeyId">User key identifier.</param>
    /// <returns>Key id of just created batch.</returns>
    Task<int> CreateBatchAsync(int userKeyId, BatchCreateArgs args);

    /// <summary>
    /// Retrieves details of batch by a batch key identifier.
    /// </summary>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <returns>BatchDetailsDto indicating the result of the operation.</returns>
    Task<BatchDetailsDto> GetBatchDetailsAsync(int batchKeyId);

    /// <summary>
    /// Approves batch.
    /// </summary>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <param name="userKeyId">User key identifier.</param>
    /// <param name="args">Batch approve arguments.</param>
    /// <returns></returns>
    Task ApproveBatchAsync(int batchKeyId, int userKeyId, BatchApproveArgs args);

    /// <summary>
    /// Disapproves batch.
    /// </summary>
    /// <param name="args">Input arguments used for disapproving batch.</param>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <param name="userKeyId">User key identifier.</param>
    /// <returns></returns>
    Task DisapproveBatchAsync(int batchKeyId, int userKeyId, BatchDisapproveArgs args);

    /// <summary>
    /// Retrieves a process by batch key identifier.
    /// </summary>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <returns>Process model.</returns>
    Task<ProcessModel> GetProcessByBatchKeyIdAsync(int batchKeyId);
}