using ProductionService.Shared.Dtos.Processes;

namespace ProductionService.Client.Services.Process;

/// <summary>
/// API service provides methods to retrieve/handle processes.
/// </summary>
public interface IProcessApiService
{
    /// <summary>
    /// Creates a new batch.
    /// </summary>
    /// <param name="args">Input arguments used for creating new batch.</param>
    /// <returns>
    /// The result of the operation. If the operation was successful, response keeps key identifier of
    /// just created batch.
    /// </returns>
    Task<int> CreateBatchAsync(BatchCreateArgs args);

    /// <summary>
    /// Approves a batch.
    /// </summary>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <param name="args">Input arguments used for approving a batch.</param>
    /// <returns>Task result.</returns>
    Task ApproveBatchAsync(int batchKeyId, BatchApproveArgs args);

    /// <summary>
    /// Disapproves a batch.
    /// </summary>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <param name="args">Input arguments used for disapproving a batch.</param>
    /// <returns>Task result.</returns>
    Task DisapproveBatchAsync(int batchKeyId, BatchDisapproveArgs args);

    /// <summary>
    /// Get batch details.
    /// </summary>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <returns>Details about the batch as BatchDetailsDto.</returns>
    Task<BatchDetailsDto> GetBatchDetailsAsync(int batchKeyId);
}