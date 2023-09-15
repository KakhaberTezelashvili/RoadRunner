using ProductionService.Shared.Dtos.Processes;

namespace ProductionService.Core.Services.Processes;

/// <summary>
/// Validator provides methods to validate processes.
/// </summary>
public interface IProcessValidator : IValidatorBase<ProcessModel>
{
    /// <summary>
    /// Validates create batch asynchronous.
    /// </summary>
    /// <param name="args">Input arguments.</param>
    Task CreateBatchValidateAsync(BatchCreateArgs args);

    /// <summary>
    /// Validates approve batch asynchronous.
    /// </summary>
    /// <param name="batchKeyId">Batch key id.</param>
    /// <param name="userKeyId">User key identifier.</param>
    /// <param name="args">Input arguments.</param>
    /// <returns>Task&lt;ProcessModel&gt;.</returns>
    Task<ProcessModel> ApproveBatchValidateAsync(int batchKeyId, int userKeyId, BatchApproveArgs args);

    /// <summary>
    /// Validates disapprove batch asynchronous.
    /// </summary>
    /// <param name="args">Input arguments.</param>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <param name="userKeyId">User key identifier.</param>
    /// <returns>Task&lt;ProcessModel&gt;.</returns>
    Task<ProcessModel> DisapproveBatchValidateAsync(int batchKeyId, int userKeyId, BatchDisapproveArgs args);
}