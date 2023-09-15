using ProductionService.Shared.Dtos.Processes;

namespace ProductionService.Core.Services.Batches;

/// <summary>
/// Validator provides methods to validate batches.
/// </summary>
public interface IBatchValidator
{
    /// <summary>
    /// Validates link units to the batch asynchronous.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="batchKeyId">Batch key identifier.</param>
    /// <param name="machineType">Machine type: sterilizer or washer.</param>
    /// <param name="userKeyId">User key identifier.</param>
    Task LinkUnitsToBatchValidateAsync(int batchKeyId, MachineType machineType, int userKeyId, BatchCreateArgs args);

    /// <summary>
    /// Validates assigning units to the batch. If machine type is <see cref="MachineType.Sterilizer"/>, 
    /// it checks units should not be linked to any sterilizer batch. 
    /// If machine type is <see cref="MachineType.Washer"/>, it checks units linked
    /// to batches should have status of <see cref="ProcessStatus.Done"/>.
    /// </summary>
    /// <param name="machineType">Machine type: sterilizer or washer.</param>
    /// <param name="unitKeyIds">List of unit identifier.</param>
    /// <param name="isSerialNumber">Is serial number.</param>
    Task AssignUnitsToSterilizeOrWashBatchValidateAsync(MachineType machineType, IEnumerable<int> unitKeyIds, bool isSerialNumber = false);

    /// <summary>
    /// Validates delete batches asynchronous.
    /// </summary>
    /// <param name="unitKeyId">Unit key id.</param>
    /// <returns></returns>
    void RemoveBatchesValidate(int unitKeyId);
}