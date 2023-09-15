using Newtonsoft.Json;
using ProductionService.Shared.Dtos.Machines;

namespace ProductionService.Core.Services.Machines;

/// <inheritdoc cref="IMachineValidator" />
public class MachineValidator : ValidatorBase<MachineModel>, IMachineValidator
{
    private readonly IMachineRepository _machineRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="MachineValidator" /> class.
    /// </summary>
    /// <param name="machineRepository">Repository provides methods to retrieve/handle machines.</param>
    public MachineValidator(IMachineRepository machineRepository) : base(machineRepository)
    {
        _machineRepository = machineRepository;
    }

    /// <inheritdoc />
    public async Task<MachineModel> GetWithMachineTypeByKeyIdValidateAsync(int keyId)
    {
        await FindByKeyIdValidateAsync(keyId);
        return await _machineRepository.GetWithMachineTypeByIdAsync(keyId);
    }

    /// <inheritdoc />
    public async Task<MachineDetailsBaseDto> MachineInfoForBatchCreatingValidateAsync(int machineKeyId, int locationKeyId)
    {
        await FindByKeyIdValidateAsync(machineKeyId);
        await FindOtherEntityByKeyIdValidateAsync<LocationModel>(locationKeyId);

        MachineDetailsDto machineDetails = await _machineRepository.GetDetailsByKeyIdAsync(machineKeyId);
        if (machineDetails == null)
            throw ArgumentNotFoundException();

        if (machineDetails.LocationKeyId != locationKeyId)
            throw InputArgumentException(InputArgumentMachineErrorCodes.MachineNotAvailableAtLocation);

        return JsonConvert.DeserializeObject<MachineDetailsBaseDto>(JsonConvert.SerializeObject(machineDetails));
    }
}