using ProductionService.Shared.Dtos.Machines;

namespace ProductionService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle machines.
/// </summary>
public interface IMachineRepository : IRepositoryBase<MachineModel>
{
    /// <summary>
    /// Retrieves machine and it's type asynchronous.
    /// </summary>
    /// <param name="machineKeyId">Machine key identifier.</param>
    /// <returns>Machine data.</returns>
    Task<MachineModel> GetWithMachineTypeByIdAsync(int machineKeyId);

    /// <summary>
    /// Retrieves all existing machines of the specified type for the specified location asynchronous.
    /// </summary>
    /// <param name="locationKeyId">Location key identifier.</param>
    /// <param name="machineType">Machine type.</param>
    /// <returns>List of machines.</returns>
    Task<IList<MachineDetailsBaseDto>> GetByLocationAndTypeAsync(int locationKeyId, MachineType? machineType = null);

    /// <summary>
    /// Retrieves the information about the specified machine asynchronous.
    /// </summary>
    /// <param name="machineKeyId">Machine key identifier.</param>
    /// <returns>Machine detail.</returns>
    Task<MachineDetailsDto> GetDetailsByKeyIdAsync(int machineKeyId);
}