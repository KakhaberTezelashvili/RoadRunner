using Newtonsoft.Json;
using ProductionService.Shared.Dtos.Programs;

namespace ProductionService.Core.Services.Programs;

/// <inheritdoc cref="IProgramValidator" />
public class ProgramValidator : ValidatorBase<ProgramModel>, IProgramValidator
{
    private readonly IProgramRepository _programRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgramValidator" /> class.
    /// </summary>
    /// <param name="programRepository">Repository provides methods to retrieve/handle programs.</param>
    public ProgramValidator(IProgramRepository programRepository) : base(programRepository)
    {
        _programRepository = programRepository;
    }

    /// <inheritdoc />
    public async Task<ProgramDetailsBaseDto> ProgramForMachineValidateAsync(int programKeyId, int machineKeyId)
    {
        await FindByKeyIdValidateAsync(programKeyId);
        await FindOtherEntityByKeyIdValidateAsync<MachineModel>(machineKeyId);

        ProgramDetailsDto programDetails = await _programRepository.GetProgramDetailsAsync(programKeyId, true);
        if (programDetails == null)
            throw ArgumentNotFoundException();

        if (programDetails.Machines == null || !programDetails.Machines.Any(m => m.KeyId == machineKeyId))
            throw ArgumentNotValidException();

        return JsonConvert.DeserializeObject<ProgramDetailsBaseDto>(JsonConvert.SerializeObject(programDetails));
    }

    /// <inheritdoc />
    public async Task<IList<ProgramDetailsBaseDto>> ProgramsByMachineValidateAsync(int machineKeyId)
    {
        await FindOtherEntityByKeyIdValidateAsync<MachineModel>(machineKeyId);
        return await _programRepository.GetProgramsByMachineAsync(machineKeyId);
    }
}