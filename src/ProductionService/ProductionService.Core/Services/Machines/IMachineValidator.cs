using ProductionService.Shared.Dtos.Machines;

namespace ProductionService.Core.Services.Machines;

/// <summary>
/// Validator provides methods to validate machines.
/// </summary>
public interface IMachineValidator : IValidatorBase<MachineModel>
{
    /// <summary>
    /// Validates machine data.
    /// </summary>
    /// <param name="keyId">Machine key identifier.</param>
    /// <returns>Task&lt;MachineModel&gt;.</returns>
    Task<MachineModel> GetWithMachineTypeByKeyIdValidateAsync(int keyId);

    /// <summary>
    /// Validates if the machine can be used at the specified location.
    /// </summary>
    /// <param name="machineKeyId">Machine key identifier.</param>
    /// <param name="locationKeyId">Location key identifier.</param>
    Task<MachineDetailsBaseDto> MachineInfoForBatchCreatingValidateAsync(int machineKeyId, int locationKeyId);
}