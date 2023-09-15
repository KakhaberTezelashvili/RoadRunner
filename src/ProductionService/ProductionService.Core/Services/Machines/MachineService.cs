using ProductionService.Shared.Dtos.Machines;

namespace ProductionService.Core.Services.Machines;

/// <inheritdoc cref="IMachineService" />
public class MachineService : IMachineService
{
    private readonly IMachineRepository _machineRepository;
    private readonly IMachineValidator _machineValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="MachineService" /> class.
    /// </summary>
    /// <param name="machineRepository">Repository provides methods to retrieve/handle machines.</param>
    /// <param name="machineValidator">Validator provides methods to validate machines.</param>
    public MachineService(IMachineRepository machineRepository, IMachineValidator machineValidator)
    {
        _machineRepository = machineRepository;
        _machineValidator = machineValidator;
    }

    /// <inheritdoc />
    public async Task<MachineModel> GetByKeyIdAsync(int keyId) =>
        await _machineValidator.GetWithMachineTypeByKeyIdValidateAsync(keyId);

    /// <inheritdoc />
    public async Task<IList<MachineDetailsBaseDto>> GetMachinesByLocationAsync(int locationKeyId, MachineType? machineType = null)
    {
        await _machineValidator.FindOtherEntityByKeyIdValidateAsync<LocationModel>(locationKeyId);
        return await _machineRepository.GetByLocationAndTypeAsync(locationKeyId, machineType);
    }

    /// <inheritdoc />
    public async Task<MachineDetailsBaseDto> GetMachineInfoForBatchCreatingAsync(int machineKeyId, int locationKeyId) => 
        await _machineValidator.MachineInfoForBatchCreatingValidateAsync(machineKeyId, locationKeyId);
}