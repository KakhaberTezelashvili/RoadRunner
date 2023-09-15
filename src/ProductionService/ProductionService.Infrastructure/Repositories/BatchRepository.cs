using ProductionService.Core.Models.Batches;

namespace ProductionService.Infrastructure.Repositories;

/// <inheritdoc cref="IBatchRepository" />
public class BatchRepository : RepositoryBase<BatchModel>, IBatchRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BatchRepository" /> class.
    /// </summary>
    /// <param name="context">EF database context.</param>
    public BatchRepository(TDocEFDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IEnumerable<BatchModel>> FindByBatchKeyIdAsync(int batchKeyId) => 
        await _context.Batches.Where(p => p.Batch == batchKeyId).ToListAsync();

    /// <inheritdoc />
    public async Task<IEnumerable<BatchModel>> FindUnFailedBatchesByBatchKeyIdAsync(int batchKeyId)
    {
        return await EntitySet
            .Where(b => b.Batch == batchKeyId && b.Status != BatchUnitStatus.UnitFailed)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<BatchModel>> FindByUnitKeyIdAndBatchTypeAsync(int unitKeyId, BatchType batchType)
    {
        return await EntitySet
            .Where(b => b.Unit == unitKeyId && b.Type == (int)batchType)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IList<BatchProcessData>> GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(IEnumerable<int> unitKeyIds, BatchType batchType)
    {
        List<BatchProcessData> batches = await (from unit in _context.Units
                                                where unitKeyIds.Contains(unit.KeyId)
                                                let batch = _context.Batches.Where(b => b.Type == (int)batchType && b.Unit == unit.KeyId).OrderByDescending(o => o.Batch).FirstOrDefault()
                                                select new BatchProcessData
                                                {
                                                    BatchKeyId = batch != null ? batch.Batch : null,
                                                    BatchType = batch != null ? (BatchType)batch.Type : null,
                                                    UnitKeyId = batch != null ? batch.Unit : null,
                                                    ProcessBatch = batch != null ? (batch.BatchProcess != null ? batch.BatchProcess.KeyId : null) : null,
                                                    ProcessStatus = batch != null ? (batch.BatchProcess != null ? batch.BatchProcess.Status : null) : null,
                                                    ProcessApprovedUserKeyId = batch != null ? (batch.BatchProcess != null ? batch.BatchProcess.ApproveUserKeyId : null) : null,
                                                    ProcessError = batch != null ? (batch.BatchProcess != null ? batch.BatchProcess.Error : null) : null,
                                                    ProgramApproval = batch != null ? (batch.BatchProcess != null
                                                        ? (batch.BatchProcess.Prog != null ? batch.BatchProcess.Prog.Approval : null) : null) : null
                                                }).ToListAsync();

        return batches;
    }
}