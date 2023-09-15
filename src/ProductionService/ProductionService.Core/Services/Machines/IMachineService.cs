using ProductionService.Shared.Dtos.Machines;

namespace ProductionService.Core.Services.Machines;

/// <summary>
/// Service provides methods to retrieve/handle machines.
/// </summary>
public interface IMachineService
{
    /// <summary>
    /// Retrieves machine by key id.
    /// </summary>
    /// <param name="keyId">Primary key of the machine.</param>
    /// <returns>Machine data.</returns>
    Task<MachineModel> GetByKeyIdAsync(int keyId);

    /// <summary>
    /// Retrieves all existing machines of the specified type for the specified location.
    /// </summary>
    /// <param name="locationKeyId">Location key id.</param>
    /// <param name="machineType">Machine type.</param>///
    /// <returns>List of machines.</returns>
    Task<IList<MachineDetailsBaseDto>> GetMachinesByLocationAsync(int locationKeyId, MachineType? machineType = null);

    /// <summary>
    /// Retrieves the machine information at the specified location.
    /// </summary>
    /// <param name="machineKeyId">Machine key id.</param>
    /// <param name="locationKeyId">Location key id.</param>
    /// <returns>
    /// <see cref="MachineDetailsBaseDto"/> as part of the response.
    /// </returns>
    Task<MachineDetailsBaseDto> GetMachineInfoForBatchCreatingAsync(int machineKeyId, int locationKeyId);
}