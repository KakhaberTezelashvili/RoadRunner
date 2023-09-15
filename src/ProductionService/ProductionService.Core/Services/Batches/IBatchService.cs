using ProductionService.Core.Models.Batches;
using ProductionService.Shared.Dtos.Processes;

namespace ProductionService.Core.Services.Batches;

/// <summary>
/// Service provides methods to retrieve/handle batches.
/// </summary>
public interface IBatchService
{
    /// <summary>
    /// Links units to the batch.
    /// </summary>
    /// <param name="args">Input arguments.</param>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <param name="machineType">Machine type: sterilizer or washer.</param>
    /// <param name="userKeyId">User key identifier.</param>
    /// <returns></returns>
    Task LinkUnitsToBatchAsync(int batchKeyId, MachineType machineType, int userKeyId, BatchCreateArgs args);

    /// <summary>
    /// Updates batches status to ok.
    /// </summary>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <returns></returns>
    Task UpdateBatchesStatusToOkAsync(int batchKeyId);

    /// <summary>
    /// Updates batches status to failed.
    /// </summary>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <returns></returns>
    Task UpdateBatchesStatusToFailedAsync(int batchKeyId);

    /// <summary>
    /// Deletes batches.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <param name="unitStatus">Unit status.</param>
    /// <returns></returns>
    Task RemoveBatchesByUnitKeyIdAndUnitStatusAsync(int unitKeyId, UnitStatus unitStatus);

    /// <summary>
    /// Gets list of batch process data based on units and batch type.
    /// </summary>
    /// <param name="unitKeyIds">Collection of Unit key identifiers.</param>
    /// <param name="batchType">Type of batch.</param>
    /// <returns>List of batch process data.</returns>
    Task<IList<BatchProcessData>> GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(IEnumerable<int> unitKeyIds, BatchType batchType);
}