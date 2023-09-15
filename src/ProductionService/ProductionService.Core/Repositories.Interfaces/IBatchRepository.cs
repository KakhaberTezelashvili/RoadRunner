using ProductionService.Core.Models.Batches;

namespace ProductionService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle batches.
/// </summary>
public interface IBatchRepository : IRepositoryBase<BatchModel>
{
    /// <summary>
    /// Returns list tracked batches asynchronous.
    /// </summary>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <returns>A task of list of batches.</returns>
    Task<IEnumerable<BatchModel>> FindByBatchKeyIdAsync(int batchKeyId);

    /// <summary>
    /// Returns list tracked not failed batches asynchronous.
    /// </summary>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <returns>A task of list of batches.</returns>
    Task<IEnumerable<BatchModel>> FindUnFailedBatchesByBatchKeyIdAsync(int batchKeyId);

    /// <summary>
    /// Returns list of tracked batches asynchronous.
    /// </summary>
    /// <param name="unitKeyId">The unit key identifier.</param>
    /// <param name="batchType">The batch type.</param>
    /// <returns>A task of list of batches.</returns>
    Task<IEnumerable<BatchModel>> FindByUnitKeyIdAndBatchTypeAsync(int unitKeyId, BatchType batchType);

    /// <summary>
    /// Returns last batch for each unit key identifier.
    /// </summary>
    /// <param name="unitKeyIds">Unit key identifier.</param>
    /// <param name="batchType">Batch type of the unit.</param>
    /// <returns>Information about batch of a unit.</returns>
    Task<IList<BatchProcessData>> GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(IEnumerable<int> unitKeyIds, BatchType batchType);
}